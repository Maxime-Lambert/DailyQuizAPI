namespace DailyQuizAPI.Features.Crosscutting.Users.Rollback;

using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public sealed class RollbackCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;
    private const string ROLLBACK_TOKEN_NAME = "Rollback";

    public async Task Handle(RollbackCommand command)
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
        var userName = claimsDict[JwtRegisteredClaimNames.Name]
            ?? throw new InvalidOperationException("Token invalide");
        var email = claimsDict[ClaimTypes.Email]
            ?? throw new InvalidOperationException("Token invalide");
        var encodedToken = claimsDict["rollbackToken"]
            ?? throw new InvalidOperationException("Token invalide");
        var originalToken = Encoding.UTF8.GetString(Convert.FromBase64String(encodedToken!));

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Token invalide");

        var result = await _userManager.VerifyUserTokenAsync(user, ROLLBACK_TOKEN_NAME, ROLLBACK_TOKEN_NAME, originalToken!)
            .ConfigureAwait(false);
        if (!result)
            throw new InvalidOperationException("Token invalide");

        user.UserName = userName;
        user.Email = email;
        user.EmailConfirmed = true;
        user.RefreshTokens.Clear();
        await _userManager.UpdateAsync(user).ConfigureAwait(false);
    }

}

