using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotPassword;

public class ForgotPasswordCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailSender<User> emailSender)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;
    private readonly IEmailSender<User> _emailSender = emailSender;

    public async Task Handle(ForgotPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        if (!user.EmailConfirmed)
        {
            throw new InvalidOperationException("User email is not confirmed.");
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);

        List<Claim> resetClaims = [
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
            new Claim("resettoken", resetToken),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: resetClaims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        var resetLink = $"{FrontEndOrigins.SUMOT}/reset-Password?token={Uri.EscapeDataString(jwtToken)}";

        await _emailSender.SendPasswordResetLinkAsync(user, command.Email, resetLink).ConfigureAwait(false);
    }
}
