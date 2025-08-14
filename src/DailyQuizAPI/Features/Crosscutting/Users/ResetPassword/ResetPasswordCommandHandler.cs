namespace DailyQuizAPI.Features.Crosscutting.Users.ResetPassword;

using DailyQuizAPI.Common.Exceptions;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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

        var userId = principal[JwtRegisteredClaimNames.NameId].ToString()
            ?? throw new InvalidOperationException("Token invalide");

        var token = principal["resettoken"].ToString();

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new NotFoundException(nameof(User), userId);

        var result = await _userManager.ResetPasswordAsync(user, token!, command.Password).ConfigureAwait(false);
        if (!result.Succeeded)
            throw new InvalidOperationException("Échec de la mise à jour du mot de passe.");
    }
}

