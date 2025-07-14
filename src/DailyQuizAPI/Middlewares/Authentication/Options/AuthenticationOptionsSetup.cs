using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Middlewares.Authentication.Options;

public sealed class AuthenticationOptionsSetup(IConfiguration configuration) : IConfigureOptions<AuthenticationOptions>
{
    private const string CONFIGURATION_SECTION = "Authentication";
    private readonly IConfiguration _configuration = configuration;

    public void Configure(AuthenticationOptions options)
    {
        _configuration.GetSection(CONFIGURATION_SECTION).Bind(options);
    }
}