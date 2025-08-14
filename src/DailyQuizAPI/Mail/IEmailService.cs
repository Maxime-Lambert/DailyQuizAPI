using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Mail;

public interface IEmailService
{
    Task SendConfirmationLinkAsync(User user, string email, string confirmationLink, FrontEndNames frontEndName);

    Task SendPasswordResetLinkAsync(User user, string email, string resetLink, FrontEndNames frontEndName);

    Task SendRollbackAsync(User user, string email, string rollbackLink, FrontEndNames frontEndName);

    Task SendInactivityWarningAsync(User user, string email);

    Task SendUserDeletedAsync(User user, string email);

    Task SendContactMessageAsync(string email, string name, string fromEmail, string message);
}
