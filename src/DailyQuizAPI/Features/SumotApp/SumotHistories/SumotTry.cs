namespace DailyQuizAPI.Features.SumotApp.SumotHistories;

public sealed class SumotTry
{
    public int Id { get; set; }
    public int SumotHistoryId { get; set; }
    public string Value { get; set; } = string.Empty;
}
