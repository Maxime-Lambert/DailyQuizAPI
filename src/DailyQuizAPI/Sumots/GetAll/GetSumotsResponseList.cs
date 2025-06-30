namespace DailyQuizAPI.Sumots.GetAll;

public sealed record GetSumotsResponseList(IReadOnlyList<GetSumotsResponse> Sumots, int DatabaseVersion);