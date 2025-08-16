using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DailyQuizAPI.Features.SumotApp.Sumots.Extract;

public sealed class ExtractSumotsCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private const string CSV_FILENAME = "Lexique-query-2025-08-15 14-58-10.csv";
    private readonly ICacheService _cacheService = cacheService;
    private readonly QuizContext _quizContext = quizContext;
    private static readonly List<string> INAPPROPRIATE_WORDS =
    [
        "ASIATE", "BOCHE", "BOCHES", "CHINO", "CHINOS", "GOGOL", "GOGOLS", "MONGOL", "GOUDOU", "GOUINE",
        "LOPES", "NABOT", "NABOTS", "NEGRE", "NEGRES", "PEDES", "PEDE", "ROMANO", "SCHLEU", "VIOLS",
        "SALOPE", "PUTES", "PUTAIN", "VIOLER", "VIOLE", "VIOLES", "VIOLEE", "ENCULE", "ENCULER", "NIQUE",
        "NIQUER", "TARBA", "TARBAS", "BATARD", "FOUTRE", "CONNE", "CONNES", "CONARD", "MERDE", "MERDES",
        "CHIANT", "BAISE", "BAISER", "BAISES", "ORGIE", "ORGIES", "SALAUD", "ZOBES"
    ];

    public async Task Handle(CancellationToken cancellationToken)
    {
        var excelFilePath = Path.Combine(AppContext.BaseDirectory, CSV_FILENAME);
        var sumotsFromLexique = await LoadSumotsFromCsvAsync(excelFilePath, cancellationToken).ConfigureAwait(false);
        var existingSumots = await _quizContext.Sumots.ToListAsync(cancellationToken).ConfigureAwait(false);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("SumotBot/1.0 (+https://sumot.app; contact@sumot.app)");
        foreach (var sumot in sumotsFromLexique)
        {
            var existing = existingSumots.FirstOrDefault(s => s.Word == sumot.Word);
            if (existing is not null)
            {
                sumot.Definition = existing.Definition;
                sumot.DefinitionWord = existing.DefinitionWord;
            }
            else
            {
#pragma warning disable CA1308 // Normaliser les chaînes en majuscules
                var lowerWord = sumot.Word.ToLowerInvariant();
#pragma warning restore CA1308 // Normaliser les chaînes en majuscules

                var attempts = new HashSet<string>
                {
                    lowerWord,
                    sumot.Word,
                    char.ToUpper(lowerWord[0], CultureInfo.InvariantCulture) + lowerWord[1..]
                };

                var baseForms = new HashSet<string>
                {
                    lowerWord,
                    char.ToUpper(lowerWord[0], CultureInfo.InvariantCulture) + lowerWord[1..]
                };

                foreach (var form in baseForms)
                {
                    foreach (var variant in GenerateAccentVariants(form))
                        attempts.Add(variant);

                    foreach (var variant in GenerateLigatureVariants(form))
                        attempts.Add(variant);
                }

                foreach (var attempt in attempts.Where(a => !string.IsNullOrEmpty(a)))
                {
                    var uri = new Uri($"https://fr.wiktionary.org/w/api.php?action=parse&page={attempt}&format=json&origin=*&prop=text");
                    HttpResponseMessage? response = null;
                    int retryCount = 0;

                    while (retryCount < 5)
                    {
                        response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);

                        if (response.StatusCode == (HttpStatusCode)429)
                        {
                            var delayMs = (int)Math.Pow(2, retryCount) * 1000;
                            Console.WriteLine($"[WARN] 429 reçu pour '{attempt}', retry dans {delayMs} ms...");
                            await Task.Delay(delayMs, cancellationToken).ConfigureAwait(false);
                            retryCount++;
                            continue;
                        }

                        break;
                    }

                    if (response is null || !response.IsSuccessStatusCode)
                        continue;

                    var json = await response.Content.ReadFromJsonAsync<JsonDocument>(cancellationToken).ConfigureAwait(false);

                    if (json is null)
                    {
                        continue;
                    }

                    if (json.RootElement.TryGetProperty("error", out var _))
                    {
                        continue;
                    }

                    if (!json.RootElement.TryGetProperty("parse", out var parse))
                    {
                        continue;
                    }

                    if (!parse.TryGetProperty("text", out var text) || !text.TryGetProperty("*", out var htmlElement))
                    {
                        continue;
                    }

                    var html = htmlElement.GetString();
                    if (string.IsNullOrWhiteSpace(html))
                    {
                        continue;
                    }

                    var doc = new HtmlDocument();
                    doc.LoadHtml(html);

                    var headings = doc.DocumentNode.SelectNodes("//div[contains(@class,'mw-parser-output')]//h2 | //h3");
                    if (headings is null)
                    {
                        continue;
                    }

                    var definition = ExtractFirstDefinition(doc);
                    if (string.IsNullOrWhiteSpace(definition))
                    {
                        continue;
                    }
                    sumot.Definition = definition;
                    sumot.DefinitionWord = attempt;
                    break;
                }
                if (string.IsNullOrEmpty(sumot.Definition))
                    Console.WriteLine("pas trouvé pour " + sumot.Word);
            }
        }
        var defs = sumotsFromLexique.Where(s => !string.IsNullOrEmpty(s.DefinitionWord));
        _quizContext.Sumots.RemoveRange(existingSumots);
        _cacheService.RemoveByPrefix("sumots:");
        await _quizContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        await _quizContext.Sumots.AddRangeAsync(defs, cancellationToken).ConfigureAwait(false);
        await _quizContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static HashSet<string> GenerateAccentVariants(string word)
    {
        var replacements = new Dictionary<char, char[]>
        {
            ['a'] = ['à', 'â'],
            ['e'] = ['é', 'è', 'ê', 'ë'],
            ['i'] = ['î', 'ï'],
            ['o'] = ['ô', 'ö'],
            ['u'] = ['ù', 'û', 'ü'],
            ['c'] = ['ç']
        };

        var variants = new HashSet<string>();
        void Recurse(char[] current, int index)
        {
            if (index >= current.Length)
            {
                variants.Add(new string(current));
                return;
            }

            var originalChar = current[index];
            Recurse(current, index + 1);

            if (replacements.TryGetValue(char.ToLowerInvariant(originalChar), out var accented))
            {
                foreach (var accent in accented)
                {
                    current[index] = accent;
                    Recurse(current, index + 1);
                    current[index] = originalChar;
                }
            }
        }

        Recurse(word.ToCharArray(), 0);
        variants.Remove(word);
        return variants;
    }
    private static string? ExtractFirstDefinition(HtmlDocument doc)
    {
        var sections = new[] { "NOM COMMUN", "ADJECTIF", "VERBE", "PRONOM", "DÉTERMINANT", "PRÉPOSITION",
            "INTERJECTION", "CONJONCTION" };

        foreach (var section in sections)
        {
            var definition = GetDefinitionForSection(doc, section);
            if (definition is not null)
                return definition;
        }

        return null;
    }
    private static string? GetDefinitionForSection(HtmlDocument doc, string section)
    {
        var output = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'mw-parser-output')]");
        if (output == null) return null;

        var nodes = output.ChildNodes
            .Where(n => n.NodeType == HtmlNodeType.Element)
            .ToList();

        bool inFrench = false;
        bool inTargetSection = false;

        foreach (var node in nodes)
        {
            if (node.GetClasses().Contains("mw-heading2"))
            {
                var h2Text = Normalize(node.InnerText);
                if (h2Text.Contains("FRANÇAIS", StringComparison.OrdinalIgnoreCase))
                {
                    inFrench = true;
                    continue;
                }

                if (inFrench) break;
            }

            if (!inFrench) continue;

            if (node.GetClasses().Contains("mw-heading3"))
            {
                var h3Text = Normalize(node.InnerText);
                if (h3Text.Contains(section, StringComparison.OrdinalIgnoreCase))
                {
                    inTargetSection = true;
                    continue;
                }

                if (inTargetSection) break;
            }

            if (!inTargetSection) continue;

            if (node.Name == "ol")
            {
                var li = node.SelectSingleNode("./li");
                if (li == null) return null;

                li.SelectNodes("./ul|./ol")?.ToList().ForEach(n => n.Remove());

                var innerHtml = string.Concat(
                    li.ChildNodes
                        .Where(n => n.Name != "ul" && n.Name != "ol")
                        .Select(n => n.OuterHtml)
                );

                var fragmentDoc = new HtmlDocument();
                fragmentDoc.LoadHtml(innerHtml);
                var anchors = fragmentDoc.DocumentNode.SelectNodes("//a[@href]");

                if (anchors != null)
                {
                    foreach (var anchor in anchors)
                    {
                        var href = anchor.GetAttributeValue("href", "");
                        if (href.StartsWith('/'))
                        {
                            anchor.SetAttributeValue("href", $"https://fr.wiktionary.org{href}");
                        }
                    }
                }

                return $"<p>{fragmentDoc.DocumentNode.InnerHtml.Trim()}</p>";
            }
        }

        return null;
    }

    private static HashSet<string> GenerateLigatureVariants(string word)
    {
        var variants = new HashSet<string>();

        if (word.Contains("oe", StringComparison.OrdinalIgnoreCase))
        {
            variants.Add(word.Replace("oe", "œ", StringComparison.OrdinalIgnoreCase));
        }

        if (word.Contains("ae", StringComparison.OrdinalIgnoreCase))
        {
            variants.Add(word.Replace("ae", "æ", StringComparison.OrdinalIgnoreCase));
        }

        return variants;
    }

    private static string Normalize(string input)
    {
        return input
            .Replace("’", "'", StringComparison.InvariantCultureIgnoreCase)
            .Replace("\u00A0", " ", StringComparison.InvariantCultureIgnoreCase)
            .Trim()
            .ToUpperInvariant();
    }

    public async Task<List<Sumot>> LoadSumotsFromCsvAsync(string csvPath, CancellationToken cancellationToken)
    {
        var lines = await File.ReadAllLinesAsync(csvPath, Encoding.UTF8, cancellationToken)
                              .ConfigureAwait(false);

        if (lines.Length <= 1)
            return [];

        var header = lines[0].Split(';');
        int idxWord = Array.IndexOf(header, "Word");
        int idxLemme = Array.IndexOf(header, "lemme");
        int idxCgram = Array.IndexOf(header, "cgram");
        int idxFreqLem = Array.IndexOf(header, "freqlemfilms2");

        var sumotsDict = new Dictionary<string, Sumot>();

        foreach (var line in lines.Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var cols = line.Split(';');
            if (cols.Length <= idxFreqLem)
                continue;

            var wordRaw = cols[idxWord].Trim();
            if (wordRaw.Contains('-', StringComparison.InvariantCulture)
                || wordRaw.Contains(' ', StringComparison.InvariantCulture)
                || INAPPROPRIATE_WORDS.Contains(wordRaw.ToUpperInvariant()))
                continue;
            var lemme = cols[idxLemme].Trim();
            var cgram = cols[idxCgram].Trim();
            var freqlemfilms2Str = cols[idxFreqLem].Trim().Replace(',', '.');

            if (!double.TryParse(freqlemfilms2Str, NumberStyles.Any, CultureInfo.InvariantCulture, out var freqlemfilms2))
                freqlemfilms2 = 0;

            // On enlève les accents + majuscules
            var word = RemoveDiacritics(wordRaw).ToUpperInvariant();

            bool isDifficult = freqlemfilms2 < 1 || ((cgram == "VER" || cgram == "AUX") && !string.Equals(wordRaw, lemme, StringComparison.OrdinalIgnoreCase));

            if (sumotsDict.TryGetValue(word, out var existing))
            {
                if (!isDifficult && existing.IsDifficult)
                {
                    existing.IsDifficult = false;
                }
            }
            else
            {
                sumotsDict[word] = new Sumot
                {
                    Word = word,
                    Day = null,
                    IsDifficult = isDifficult
                };
            }
        }

        return sumotsDict.Values.ToList();
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var regex = new Regex(@"\p{Mn}", RegexOptions.Compiled);
        return regex.Replace(normalized, string.Empty);
    }
}