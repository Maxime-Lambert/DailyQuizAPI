using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotPassword;

public class ForgotPasswordCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailService emailService)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly AuthenticationOptions _options = options.Value;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(ForgotPasswordCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email).ConfigureAwait(false);
        if (user is null || !user.EmailConfirmed)
        {
            return;
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user).ConfigureAwait(false);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(resetToken));

        List<Claim> resetClaims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("resettoken", encodedToken),
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
        var resetLink = $"{FrontEndOrigins.SUMOT}/resetpassword?token={Uri.EscapeDataString(jwtToken)}";

        await _emailService.SendPasswordResetLinkAsync(user, command.Email, resetLink, command.FrontEndName).ConfigureAwait(false);
    }
}
