namespace DailyQuizAPI.Features.Crosscutting.Users.GetOne;

public sealed record GetUserResponse(string UserId, string UserName, string? Email, TypeClavier TypeClavier, ModeDaltonien ModeDaltonien);
