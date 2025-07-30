namespace DailyQuizAPI.Features.Crosscutting.Users.Rollback;

using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        Dictionary<string, object> principal = [];
        try
        {
            var validateToken = await handler.ValidateTokenAsync(command.Token, parameters).ConfigureAwait(false);
            if (validateToken.IsValid)
            {
                principal = (Dictionary<string, object>)validateToken.Claims;
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Token invalide ou expiré.");
        }

        var userId = principal[JwtRegisteredClaimNames.NameId].ToString();
        var username = principal[JwtRegisteredClaimNames.Name].ToString();
        var email = principal[JwtRegisteredClaimNames.Email].ToString();
        var token = principal["rollbackToken"].ToString();

        var user = await _userManager.FindByIdAsync(userId!).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Utilisateur introuvable.");

        var result = await _userManager.VerifyUserTokenAsync(user, ROLLBACK_TOKEN_NAME, ROLLBACK_TOKEN_NAME, token!).ConfigureAwait(false);
        if (!result)
            throw new InvalidOperationException("Token invalide ou expiré.");
        user.UserName = username;
        user.Email = email;
        user.EmailConfirmed = true;
        user.RefreshTokens.Clear();
        await _userManager.UpdateAsync(user).ConfigureAwait(false);
    }
}

