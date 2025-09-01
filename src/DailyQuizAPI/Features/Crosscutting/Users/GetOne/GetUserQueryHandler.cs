using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.Users.GetOne;

public sealed class GetUserQueryHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<GetUserResponse> Handle(ClaimsPrincipal userClaims)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

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
