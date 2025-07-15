using Microsoft.Extensions.Caching.Memory;

namespace DailyQuizAPI.Features.Crosscutting.Caching;

public sealed class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    private static readonly HashSet<string> KNOWN_KEYS = [];

    public void RemoveByPrefix(string prefix)
    {
        var toRemove = KNOWN_KEYS.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
        foreach (var key in toRemove)
        {
            memoryCache.Remove(key);
            KNOWN_KEYS.Remove(key);
        }
    }

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (memoryCache.TryGetValue(key, out var cached) && cached is T value)
            return value;

        var result = await factory().ConfigureAwait(false);

        memoryCache.Set(key, result, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        });
        KNOWN_KEYS.Add(key);

        return result;
    }

    public void Remove(string key) => memoryCache.Remove(key);
}


