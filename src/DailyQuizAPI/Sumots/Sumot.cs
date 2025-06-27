namespace DailyQuizAPI.Sumots;

public sealed class Sumot
{
    public int Id { get; set; }

    public string? Word { get; set; }

    public DateOnly? Day { get; set; }
}
