using DailyQuizAPI.Features.Crosscutting.Users;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories;

public sealed class SumotHistory
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = default!;

    public bool Won { get; set; }

    private readonly List<SumotTry> _tries = [];

    public IReadOnlyCollection<SumotTry> Tries => _tries.AsReadOnly();

    public void ReplaceTries(IEnumerable<string> newTries)
    {
        _tries.Clear();
        foreach (var t in newTries)
        {
            _tries.Add(new SumotTry { Value = t });
        }
    }
}

