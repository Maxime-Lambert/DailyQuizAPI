namespace DailyQuizAPI.Features.Crosscutting.Caching.ClearAllCache;

public class ClearAllCacheCommandHandler(ICacheService cacheService)
{
    private readonly ICacheService _cacheService = cacheService;

    public Task Handle()
    {
        _cacheService.RemoveByPrefix("");
        return Task.CompletedTask;
    }
}