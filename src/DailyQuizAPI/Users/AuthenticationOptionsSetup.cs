using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Users;

public sealed class AuthenticationOptionsSetup(IConfiguration configuration) : IConfigureOptions<AuthenticationOptions>
{
    private const string _configurationSection = "Jwt";
    private readonly IConfiguration _configuration = configuration;

    public void Configure(AuthenticationOptions options)
    {
        _configuration.GetSection(_configurationSection).Bind(options);
    }
}