namespace DailyQuizAPI.Features.Crosscutting.Users.Create;

using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CreateUserCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailService emailService)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(CreateUserCommand request)
    {
        var user = new User
        {
            UserName = request.UserName
        };

        if (!string.IsNullOrEmpty(request.Email))
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email).ConfigureAwait(false);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"An account with email '{request.Email}' already exists.");
            }
            user.Email = request.Email;
        }

        var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        if (string.IsNullOrEmpty(request.Email))
        {
            return;
        }

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

        List<Claim> claims = [
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim("conftoken", confirmationToken),
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
        var confirmationLink = $"{FrontEndOrigins.SUMOT}/confirm-email?token={Uri.EscapeDataString(jwtToken)}";
        await _emailService.SendConfirmationLinkAsync(user, request.Email, confirmationLink, request.FrontEndName).ConfigureAwait(false);
    }
}
