namespace DailyQuizAPI.Features.SumotApp.SumotStats;

public sealed class SumotStat
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public bool IsMobile { get; set; }

    public int Visits { get; set; }

    public int Attempts { get; set; }

    public int Finishes { get; set; }
}
