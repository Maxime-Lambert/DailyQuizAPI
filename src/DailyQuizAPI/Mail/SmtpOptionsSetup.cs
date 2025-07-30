using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Mail;

public sealed class SmtpOptionsSetup(IConfiguration configuration) : IConfigureOptions<SmtpOptions>
{
    private const string CONFIGURATION_SECTION = "Smtp";
    private readonly IConfiguration _configuration = configuration;

    public void Configure(SmtpOptions options)
    {
        _configuration.GetSection(CONFIGURATION_SECTION).Bind(options);
    }
}