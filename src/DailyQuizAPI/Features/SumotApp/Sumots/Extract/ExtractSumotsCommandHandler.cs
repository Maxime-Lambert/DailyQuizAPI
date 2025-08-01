using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using HtmlAgilityPack;
using System.Globalization;
using System.Text.Json;

namespace DailyQuizAPI.Features.SumotApp.Sumots.Extract;

public sealed class ExtractSumotsCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private const string ODS_FILENAME = "ods6.txt";
    private readonly ICacheService _cacheService = cacheService;
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(ExtractSumotsCommand request, CancellationToken cancellationToken)
    {
        var sumotsFilePath = Path.Combine(AppContext.BaseDirectory, ODS_FILENAME);
        var words = await File.ReadAllLinesAsync(sumotsFilePath, cancellationToken).ConfigureAwait(false);
        var sumots = words.Where(w => w.Length == request.WordLength)
                         .Distinct()
                         .Where(w => !_quizContext.Sumots.Any(s => s.Word! == w))
                         .Select(w => new Sumot { Word = w.Trim().ToUpperInvariant(), Day = null })
                         .ToList();

        if (sumots.Count == 0)
        {
            _cacheService.RemoveByPrefix("sumots:");
        }

        using var client = new HttpClient();
        foreach (var sumot in sumots)
        {
            var lowerWord = sumot.Word.ToLowerInvariant();

            var attempts = new HashSet<string>();

            var baseForms = new HashSet<string>
            {
                lowerWord
            };

            if (lowerWord.EndsWith('s'))
                baseForms.Add(lowerWord[..^1]);

            if (lowerWord.EndsWith('x'))
                baseForms.Add(lowerWord[..^1]);

            if (lowerWord.EndsWith('t'))
                baseForms.Add(lowerWord[..^1] + 'r');

            baseForms.Add(lowerWord + 's');

            baseForms.Select(baseForms => baseForms.ToUpperInvariant())
                     .ToList()
                     .ForEach(upperForm => baseForms.Add(upperForm));

            baseForms.Add(char.ToUpper(lowerWord[0], CultureInfo.InvariantCulture) + lowerWord[1..]);

            if (lowerWord.EndsWith('s'))
                baseForms.Add(char.ToUpper(lowerWord[0], CultureInfo.InvariantCulture) + lowerWord[1..^1]);

            foreach (var form in baseForms)
            {
                attempts.Add(form);

                foreach (var variant in GenerateAccentVariants(form))
                    attempts.Add(variant);

                foreach (var variant in GenerateLigatureVariants(form))
                    attempts.Add(variant);
            }

            foreach (var attempt in attempts.Where(a => !string.IsNullOrEmpty(a)))
            {
                var uri = new Uri($"https://fr.wiktionary.org/w/api.php?action=parse&page={attempt}&format=json&origin=*&prop=text");
                var response = await client.GetAsync(uri, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    continue;
                }

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
                if (headings == null)
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
        }
        _quizContext.Sumots.AddRange(sumots.Where(s => !string.IsNullOrEmpty(s.DefinitionWord)));
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
}