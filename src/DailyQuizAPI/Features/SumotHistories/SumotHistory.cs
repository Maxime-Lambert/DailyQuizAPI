using DailyQuizAPI.Features.Users;
using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotHistories;

public sealed class SumotHistory
{
    public int Id { get; set; }

    public DateOnly Day { get; set; }

    public string Word { get; set; } = string.Empty;

    public int? Ranking { get; set; }

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = default!;

    private readonly List<string> _tries = [];

    public IReadOnlyCollection<string> Tries => _tries.AsReadOnly();

    public void AddTries(Collection<string> tries)
    {
        _tries.AddRange(tries);
    }

    public void ClearTries() => _tries.Clear();

}
