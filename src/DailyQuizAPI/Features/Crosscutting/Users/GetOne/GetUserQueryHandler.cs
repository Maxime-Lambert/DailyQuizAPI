using DailyQuizAPI.Common.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Crosscutting.Users.GetOne;

public sealed class GetUserQueryHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<GetUserResponse> Handle(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new NotFoundException(nameof(User), userId);

        return new GetUserResponse(user.Id,
            user.UserName!,
            user.Email,
            Enum.GetName(user.KeyboardLayout)!,
            Enum.GetName(user.ColorblindMode)!,
            Enum.GetName(user.SmartKeyboardType)!,
            user.EmailConfirmed,
            user.PlaysWithDifficultWords);
    }
}
