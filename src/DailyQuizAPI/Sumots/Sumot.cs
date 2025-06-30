namespace DailyQuizAPI.Sumots;

public sealed class Sumot
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public DateOnly? Day { get; set; }
}
