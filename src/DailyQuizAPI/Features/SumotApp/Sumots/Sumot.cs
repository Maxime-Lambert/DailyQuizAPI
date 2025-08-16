namespace DailyQuizAPI.Features.SumotApp.Sumots;

public sealed class Sumot
{
    public int Id { get; set; }

    public string Word { get; set; } = string.Empty;

    public DateOnly? Day { get; set; }

    public string Definition { get; set; } = string.Empty;

    public string DefinitionWord { get; set; } = string.Empty;

    public bool IsDifficult { get; set; }
}
