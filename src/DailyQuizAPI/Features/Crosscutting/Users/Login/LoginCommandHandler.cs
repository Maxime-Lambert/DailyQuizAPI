using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.Login;

public sealed class LoginCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<LoginResponse> Handle(LoginCommand request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false)
            ?? throw new InvalidOperationException("La combinaison nom d'utilisateur / mot de passe est incorrecte");

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password).ConfigureAwait(false);
        if (!isPasswordValid)
            throw new InvalidOperationException("La combinaison nom d'utilisateur / mot de passe est incorrecte");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, request.UserName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(90)
        };

        user.RefreshTokens.Add(refreshToken);
        user.LastLogin = DateOnly.FromDateTime(DateTime.UtcNow);

        await _userManager.UpdateAsync(user).ConfigureAwait(false);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new LoginResponse(jwt, refreshToken.Token);
    }
}

