namespace DailyQuizAPI.Features.Sumots.GetAll;

public sealed record GetSumotsResponseList(IReadOnlyList<GetSumotsResponse> Sumots, int DatabaseVersion);