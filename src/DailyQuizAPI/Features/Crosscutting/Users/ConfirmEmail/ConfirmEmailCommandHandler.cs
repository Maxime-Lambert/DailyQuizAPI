using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        IDictionary<string, object> principal = new Dictionary<string, object>();
        try
        {
            var validateToken = await handler.ValidateTokenAsync(command.Token, parameters).ConfigureAwait(false);
            if (validateToken.IsValid)
            {
                principal = validateToken.Claims;
            }
        }
        catch (Exception)
        {
            throw new InvalidOperationException("Token invalide ou expiré.");
        }

        var userId = principal[JwtRegisteredClaimNames.NameId].ToString();
        var token = principal["conftoken"].ToString();

        var user = await _userManager.FindByIdAsync(userId!).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Utilisateur introuvable.");

        var result = await _userManager.ConfirmEmailAsync(user, token!).ConfigureAwait(false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Échec de la confirmation de l’e-mail.");
    }
}

