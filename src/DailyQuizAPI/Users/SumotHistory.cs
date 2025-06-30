namespace DailyQuizAPI.Users;

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

    public void AddTry(string attempt) => _tries.Add(attempt);

    public void ClearTries() => _tries.Clear();

}
