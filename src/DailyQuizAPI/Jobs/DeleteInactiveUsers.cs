using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace DailyQuizAPI.Jobs;

public sealed class DeleteInactiveUsers(QuizContext db, ILogger<DeleteInactiveUsers> logger, IEmailService emailService)
{
    public async Task RunAsync(CancellationToken ct = default)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        var eighteenMonthsAgo = now.AddMonths(-18);
        var twoYearsAgo = now.AddYears(-2);
 
        var usersToDelete = await db.Users
            .Where(u => u.LastLogin != null && u.LastLogin <= twoYearsAgo)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        if (usersToDelete.Count != 0)
        {
            db.Users.RemoveRange(usersToDelete);
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
            logger.LogUsersDeleted(usersToDelete.Count, now);
        }

        var usersToWarn = await db.Users
            .Where(u => u.LastLogin != null &&
                        u.LastLogin == eighteenMonthsAgo)
            .ToListAsync(ct)
            .ConfigureAwait(false);

        foreach (var user in usersToWarn)
        {
            if (string.IsNullOrWhiteSpace(user.Email)) continue;

            try
            {
                await emailService.SendInactivityWarningAsync(user, user.Email).ConfigureAwait(false);
                logger.LogInactivityUserWarning(user.Email, now);
            }
            catch (SmtpException smtpEx)
            {
                logger.LogEmailSendException(user.Email, smtpEx.Message);
            }
            catch (InvalidOperationException invalidOpEx)
            {
                logger.LogEmailSendException(user.Email, invalidOpEx.Message);
            }
        }
    }
}
