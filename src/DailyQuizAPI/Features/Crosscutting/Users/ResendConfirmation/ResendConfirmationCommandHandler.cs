using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DailyQuizAPI.Features.Crosscutting.Users.ResendConfirmation;

public class ResendConfirmationCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailService emailService)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(ResendConfirmationCommand command)
    {
        if (string.IsNullOrEmpty(command.Email))
        {
            throw new InvalidOperationException("L'email est obligatoire");
        }

        var user = await _userManager.FindByEmailAsync(command.Email).ConfigureAwait(false);

        if (user is null)
        {
            return;
        }

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);
        var encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(confirmationToken));

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("conftoken", encodedToken),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        var confirmationLink = $"{FrontEndOrigins.SUMOT}/confirmemail?token={Uri.EscapeDataString(jwtToken)}";
        await _emailService.SendConfirmationLinkAsync(user, command.Email, confirmationLink, command.FrontEndName).ConfigureAwait(false);
    }
}