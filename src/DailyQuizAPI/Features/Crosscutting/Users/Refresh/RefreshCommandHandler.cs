using DailyQuizAPI.Exceptions;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.Refresh;

public sealed class RefreshCommandHandler(UserManager<User> userManager, IOptions<AuthenticationOptions> options)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<RefreshResponse> Handle(RefreshCommand command, CancellationToken ct)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == command.RefreshToken), cancellationToken: ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

        var token = user.RefreshTokens.SingleOrDefault(t => t.Token == command.RefreshToken);

        if (token is null || !token.IsActive)
            throw new InvalidOperationException("Connexion invalide");

        token.RevokedAt = DateTime.UtcNow;

        RefreshToken newRefreshToken = new()
        {
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(90)
        };

        token.ReplacedByToken = newRefreshToken.Token;
        user.RefreshTokens.Add(newRefreshToken);
        user.LastLogin = DateOnly.FromDateTime(DateTime.UtcNow);

        await _userManager.UpdateAsync(user).ConfigureAwait(false);

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        return new RefreshResponse(jwt, newRefreshToken.Token);
    }
}
