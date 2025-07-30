using DailyQuizAPI.Features.Crosscutting.Users;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Mail;

public interface IEmailService : IEmailSender<User>
{
    Task SendEmailAsync(string target, string subject, string plainTextContent, string? htmlContent = null);

    Task SendRollbackAsync(User user, string email, string rollbackLink);

    Task SendInactivityWarningAsync(User user, string email);
}
