using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.IntegrationTests.MockServices;

public class FakeEmailService : IEmailService
{
    public Task SendAsync(string to, string subject, string body)
    {
        return Task.CompletedTask;
    }

    public Task SendUsernameAsync(User user, string email, FrontEndNames frontEndName)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendConfirmationLinkAsync(User user, string email, string confirmationLink, FrontEndNames frontEndName)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendContactMessageAsync(string email, string name, string fromEmail, string message)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendInactivityWarningAsync(User user, string email)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendPasswordResetLinkAsync(User user, string email, string resetLink, FrontEndNames frontEndName)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendRollbackAsync(User user, string email, string rollbackLink, FrontEndNames frontEndName)
    {
        return Task.CompletedTask;
    }

    Task IEmailService.SendUserDeletedAsync(User user, string email)
    {
        return Task.CompletedTask;
    }
}
