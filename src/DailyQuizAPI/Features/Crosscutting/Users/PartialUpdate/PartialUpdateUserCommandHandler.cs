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

    public async Task Handle(PartialUpdateUserCommand command, ClaimsPrincipal userClaims)
    {
        var userId = userClaims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

        if (!string.IsNullOrWhiteSpace(command.UserName))
        {
            if (command.UserName.Length > 19)
                throw new InvalidOperationException("Les pseudos ne peuvent pas dépasser 19 caractères");
            user.UserName = command.UserName;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var needsRollbackEmail = false;
        var previousEmail = user.Email;

        if (command.Email is not null)
        {
            user.EmailConfirmed = false;
            user.Email = command.Email;
            if(!string.IsNullOrWhiteSpace(command.Email))
            {
                var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
                var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken));

                List<Claim> claims = [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim("conftoken", encodedToken),
                ];

                var token = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
                var confirmationLink = $"{FrontEndOrigins.SUMOT}/confirmemail?token={Uri.EscapeDataString(jwtToken)}";
                await _emailService.SendConfirmationLinkAsync(user, user.Email, confirmationLink, command.FrontEndName).ConfigureAwait(false);
            }
            if (!string.IsNullOrEmpty(previousEmail))
            {
                needsRollbackEmail = true;
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
                throw new InvalidOperationException("Pour modifier le mot de passe, il faut l'ancien mot de passe");
            var passwordCheck = await _userManager.CheckPasswordAsync(user, command.LastPassword).ConfigureAwait(false);
            if (!passwordCheck)
                throw new InvalidOperationException("L'ancien mot de passe est incorrect");
            if (command.NewPassword.Length > 20)
            {
                throw new InvalidOperationException("Les mots de passe ne peuvent pas dépasser 20 caractères");
            }
            if (command.NewPassword.Length < 8)
            {
                throw new InvalidOperationException("Les mots de passe ne peuvent pas faire moins de 8 caractères");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
            await _userManager.ResetPasswordAsync(user, token, command.NewPassword).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(previousEmail))
            {
                needsRollbackEmail = true;
            }
        }

        if (needsRollbackEmail)
            await SendRollbackToken(command, user, creds).ConfigureAwait(false);

        await _userManager.UpdateAsync(user).ConfigureAwait(false);
    }

    private async Task SendRollbackToken(PartialUpdateUserCommand command, User user, SigningCredentials creds)
    {
        if (user.Email is null)
            return;

        var rollbackToken = await _userManager.GenerateUserTokenAsync(user, ROLLBACK_TOKEN_NAME, ROLLBACK_TOKEN_NAME).ConfigureAwait(false);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(rollbackToken));

        List<Claim> rollbackclaims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("rollbackToken", encodedToken),
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
