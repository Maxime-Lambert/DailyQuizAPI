using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;

    public async Task Handle(ConfirmEmailCommand command)
    {
        var handler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _options.Issuer,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = key
        };

        var validateToken = await handler.ValidateTokenAsync(command.Token, parameters).ConfigureAwait(false);
        if (!validateToken.IsValid)
            throw new InvalidOperationException("Token invalide ou expiré");

        var claimsDict = validateToken.Claims.ToDictionary(c => c.Key, c => c.Value.ToString());

        var userId = claimsDict[ClaimTypes.NameIdentifier];
        var encodedToken = claimsDict["conftoken"];
        var originalToken = Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken!));
        var user = await _userManager.FindByIdAsync(userId!).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Token invalide");

        if (user.EmailConfirmed)
        {
            return;
        }

        await _userManager.ConfirmEmailAsync(user, originalToken).ConfigureAwait(false);
    }
}

