using DailyQuizAPI.Features.Crosscutting.Users;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Mail;

public static class SmtpEmailSetup
{
    public static IServiceCollection AddSmtpEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpOptions>(configuration.GetSection("Smtp"));
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<IEmailSender<User>, SmtpEmailService>();
        return services;
    }
}

