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
            ?? throw new InvalidOperationException("User not found");

        if (!string.IsNullOrWhiteSpace(command.UserName))
            user.UserName = command.UserName;

        if (!string.IsNullOrWhiteSpace(command.Email))
        {
            user.EmailConfirmed = false;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            if (!string.IsNullOrEmpty(user.Email))
            {
                var rollbackToken = await _userManager.GenerateUserTokenAsync(user, ROLLBACK_TOKEN_NAME, ROLLBACK_TOKEN_NAME).ConfigureAwait(false);

                List<Claim> rollbackclaims = [
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                    new Claim(JwtRegisteredClaimNames.Name, user.UserName!),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
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
                await _emailService.SendRollbackAsync(user, user.Email, rollbackLink).ConfigureAwait(false);
            }
            user.Email = command.Email;
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

            List<Claim> claims = [
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
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
            await _emailService.SendConfirmationLinkAsync(user, user.Email, confirmationLink).ConfigureAwait(false);
        }

        if (command.ColorblindMode is not null)
            user.ColorblindMode = command.ColorblindMode.Value;

        if (command.KeyboardLayout is not null)
            user.KeyboardLayout = command.KeyboardLayout.Value;

        if (command.SmartKeyboardType is not null)
            user.SmartKeyboardType = command.SmartKeyboardType.Value;

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
}
