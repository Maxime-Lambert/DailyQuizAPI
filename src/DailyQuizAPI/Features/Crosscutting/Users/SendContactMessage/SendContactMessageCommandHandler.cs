using DailyQuizAPI.Mail;
using DailyQuizAPI.Middlewares;

namespace DailyQuizAPI.Features.Crosscutting.Users.SendContactMessage;

public class SendContactMessageCommandHandler(IEmailService emailService)
{
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(SendContactMessageCommand request)
    {
        switch (request.FrontEndName)
        {
            case FrontEndNames.SUMOT:
                await _emailService.SendContactMessageAsync("contact@sumot.app", request.Name, request.Email, request.Message).ConfigureAwait(false);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}

