namespace DailyQuizAPI.Features.Crosscutting.Users.Create;

using DailyQuizAPI.Middlewares.Authentication.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class CreateUserCommandHandler(IOptions<AuthenticationOptions> options, UserManager<User> userManager, IEmailSender<User> emailSender)
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailSender<User> _emailSender = emailSender;
    private const string FRONTEND_ORIGIN = "https://icy-bush-06f104403.2.azurestaticapps.net/";

    public async Task Handle(CreateUserCommand request)
    {
        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email
        };

        if (string.IsNullOrEmpty(request.Email))
        {
            var result = await _userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }
        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user).ConfigureAwait(false);

        List<Claim> claims = [
            new Claim(JwtRegisteredClaimNames.NameId, user.Id),
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
        var confirmationLink = $"{FRONTEND_ORIGIN}/confirm-email?token={Uri.EscapeDataString(jwtToken)}";
        await _emailSender.SendConfirmationLinkAsync(user, request.Email, confirmationLink).ConfigureAwait(false);
    }
}
