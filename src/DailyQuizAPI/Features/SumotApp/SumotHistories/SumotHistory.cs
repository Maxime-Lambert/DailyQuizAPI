using DailyQuizAPI.Features.Crosscutting.Users;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories;

public sealed class SumotHistory
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public User User { get; set; } = default!;

    public bool Won { get; set; }

    public ICollection<SumotTry> Tries { get; private set; } = [];

    public void ReplaceTries(IEnumerable<string> newTries)
    {
        Tries.Clear();
        foreach (var t in newTries)
        {
            Tries.Add(new SumotTry { Value = t });
        }
    }
}

