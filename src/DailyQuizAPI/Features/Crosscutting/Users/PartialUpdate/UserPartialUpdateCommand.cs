using DailyQuizAPI.Features.Crosscutting.Users;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed record UserPartialUpdateCommand(
    string? UserName,
    string? Email,
    string? Password,
    ModeDaltonien? ModeDaltonien,
    TypeClavier? TypeClavier
);
