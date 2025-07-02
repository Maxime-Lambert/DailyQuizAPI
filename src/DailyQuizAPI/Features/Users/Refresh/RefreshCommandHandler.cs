using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Users.Refresh;

public sealed class RefreshCommandHandler(UserManager<User> userManager, IOptions<AuthenticationOptions> options)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<RefreshResponse> Handle(RefreshCommand command, CancellationToken ct)
    {
        var user = await _userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == command.RefreshToken), cancellationToken: ct).ConfigureAwait(false)
             ?? throw new InvalidOperationException("User not found.");

        var token = user.RefreshTokens.SingleOrDefault(t => t.Token == command.RefreshToken);

        if (token is null || !token.IsActive)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = command.IpAdress;

        RefreshToken newRefreshToken = new()
        {
            Token = Guid.NewGuid().ToString("N"),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = command.IpAdress,
        };

        token.ReplacedByToken = newRefreshToken.Token;
        user.RefreshTokens.Add(newRefreshToken);

        await _userManager.UpdateAsync(user).ConfigureAwait(false);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

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
