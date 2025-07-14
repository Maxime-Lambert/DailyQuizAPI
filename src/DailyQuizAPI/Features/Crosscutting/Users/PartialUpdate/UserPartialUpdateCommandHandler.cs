using DailyQuizAPI.Features.Crosscutting.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed class UserPartialUpdateCommandHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task Handle(UserPartialUpdateCommand command, string userId, ClaimsPrincipal userPrincipal)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found");

        if (!string.IsNullOrWhiteSpace(command.UserName))
            user.UserName = command.UserName;

        if (!string.IsNullOrWhiteSpace(command.Email))
            user.Email = command.Email;

        if (command.ModeDaltonien is not null)
            user.ModeDaltonien = command.ModeDaltonien.Value;

        if (command.TypeClavier is not null)
            user.TypeClavier = command.TypeClavier.Value;

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var result = await _userManager.ResetPasswordAsync(user, token, command.Password).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!updateResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
    }
}
