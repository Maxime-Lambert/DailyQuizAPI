using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;

namespace DailyQuizAPI.Features.SumotApp.Sumots.Extract;

public sealed class ExtractSumotsCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(ExtractSumotsCommand request, CancellationToken cancellationToken)
    {
        var sumotsFilePath = Path.Combine(AppContext.BaseDirectory, "ods6.txt");
        var words = await File.ReadAllLinesAsync(sumotsFilePath, cancellationToken).ConfigureAwait(false);
        var sumots = words.Where(w => w.Length == request.WordLength)
                         .Distinct()
                         .Where(w => !_quizContext.Sumots.Any(s => s.Word! == w))
                         .Select(w => new Sumot { Word = w.Trim().ToUpperInvariant(), Day = null });
        if (!sumots.Any())
        {
            _cacheService.RemoveByPrefix("sumots:");
            _quizContext.Sumots.AddRange(sumots);
            await _quizContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}