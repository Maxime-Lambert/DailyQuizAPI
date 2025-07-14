namespace DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

public sealed record GetSumotsResponseList(IReadOnlyList<GetSumotsResponse> Sumots, int DatabaseVersion);