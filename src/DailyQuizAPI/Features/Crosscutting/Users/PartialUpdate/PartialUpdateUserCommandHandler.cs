using DailyQuizAPI.Common.Exceptions;
using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public sealed class PartialUpdateUserCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailService emailService)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;
    private readonly IEmailService _emailService = emailService;
    private const string ROLLBACK_TOKEN_NAME = "Rollback";

    public async Task Handle(PartialUpdateUserCommand command, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!string.IsNullOrWhiteSpace(command.Username))
            user.UserName = command.Username;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        if (command.Email is not null)
        {
            await SendRollbackToken(command, user, creds).ConfigureAwait(false);

            user.EmailConfirmed = false;
            user.Email = command.Email;

            if (user.Email.Length > 0)
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

                List<Claim> claims = [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("conftoken", confirmationToken),
                ];

                var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                var confirmationLink = $"{FrontEndOrigins.SUMOT}/confirm-email?token={Uri.EscapeDataString(jwtToken)}";
                await _emailService.SendConfirmationLinkAsync(user, user.Email, confirmationLink, command.FrontEndName).ConfigureAwait(false);
            }
        }

        if (command.ColorblindMode is not null)
            user.ColorblindMode = command.ColorblindMode.Value;

        if (command.KeyboardLayout is not null)
            user.KeyboardLayout = command.KeyboardLayout.Value;

        if (command.SmartKeyboardType is not null)
            user.SmartKeyboardType = command.SmartKeyboardType.Value;

        if (command.PlaysWithDifficultWords is not null)
        {
            user.PlaysWithDifficultWords = command.PlaysWithDifficultWords.Value;
        }

        if (!string.IsNullOrWhiteSpace(command.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(command.LastPassword))
                throw new InvalidOperationException("Last password is required to change the password.");
            var passwordCheck = await _userManager.CheckPasswordAsync(user, command.LastPassword).ConfigureAwait(false);
            if (!passwordCheck)
                throw new InvalidOperationException("Last password is incorrect.");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            var result = await _userManager.ResetPasswordAsync(user, token, command.NewPassword).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        var updateResult = await _userManager.UpdateAsync(user).ConfigureAwait(false);
        if (!updateResult.Succeeded)
            throw new InvalidOperationException(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
    }

    private async Task SendRollbackToken(PartialUpdateUserCommand command, User user, SigningCredentials creds)
    {
        if (user.Email is null)
            return;

        var rollbackToken = await _userManager.GenerateUserTokenAsync(user, ROLLBACK_TOKEN_NAME, ROLLBACK_TOKEN_NAME).ConfigureAwait(false);

        List<Claim> rollbackclaims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("rollbackToken", rollbackToken),
            ];

        var rollbacktoken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: rollbackclaims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var rollbackjwtToken = new JwtSecurityTokenHandler().WriteToken(rollbacktoken);
        var rollbackLink = $"{FrontEndOrigins.SUMOT}/rollback?token={Uri.EscapeDataString(rollbackjwtToken)}";
        await _emailService.SendRollbackAsync(user, user.Email, rollbackLink, command.FrontEndName).ConfigureAwait(false);
    }
}
