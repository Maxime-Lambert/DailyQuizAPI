using DailyQuizAPI.Mail;
using Microsoft.AspNetCore.Identity;

namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotUsername;

public class ForgotUsernameCommandHandler(UserManager<User> userManager, IEmailService emailService)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(ForgotUsernameCommand command)
    {
        var user = await _userManager.FindByEmailAsync(command.Email).ConfigureAwait(false);

        if (user is null || !user.EmailConfirmed)
        {
            return;
        }

        await _emailService.SendUsernameAsync(user, command.Email, command.FrontEndName).ConfigureAwait(false);
    }
}
