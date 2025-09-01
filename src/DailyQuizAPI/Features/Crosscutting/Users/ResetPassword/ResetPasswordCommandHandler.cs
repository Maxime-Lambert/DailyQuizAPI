namespace DailyQuizAPI.Features.Crosscutting.Users.ResetPassword;

using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class ResetPasswordCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;

    public async Task Handle(ResetPasswordCommand command)
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

        var userId = claimsDict[ClaimTypes.NameIdentifier]
            ?? throw new InvalidOperationException("Token invalide");

        var encodedToken = claimsDict["resettoken"]
            ?? throw new InvalidOperationException("Token invalide");
        var originalToken = Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken!));

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Token invalide");

        await _userManager.ResetPasswordAsync(user, originalToken, command.Password).ConfigureAwait(false);
    }
}

