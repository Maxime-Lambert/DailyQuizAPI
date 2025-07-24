using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Crosscutting.Users.GetOne;

public sealed class GetUserQueryHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<GetUserResponse> Handle(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found");

        return new GetUserResponse(user.Id, user.UserName!, user.Email, user.TypeClavier, user.ModeDaltonien);
    }
}
