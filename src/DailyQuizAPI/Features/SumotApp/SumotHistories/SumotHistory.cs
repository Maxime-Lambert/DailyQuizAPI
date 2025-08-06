using DailyQuizAPI.Features.Crosscutting.Users;
using System.Collections.ObjectModel;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories;

public sealed class SumotHistory
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public int? Ranking { get; set; }

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = default!;

    private readonly List<string> _tries = [];

    public bool IsFinished => _tries.Count > 0 && _tries[^1] == Word;

    public IReadOnlyCollection<string> Tries => _tries.AsReadOnly();

    public void AddTries(Collection<string> tries)
    {
        _tries.AddRange(tries);
    }

    public void ClearTries() => _tries.Clear();

}
