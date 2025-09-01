using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Features.Crosscutting.Users.Logout;

public sealed class LogoutCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(LogoutCommand command)
    {
        var user = await _quizContext.Users
            .Include(u => u.RefreshTokens)
            .Where(u => u.RefreshTokens.Any(rt => rt.Token == command.RefreshToken))
            .FirstOrDefaultAsync()
            .ConfigureAwait(false);

        if (user == null)
            return;

        var refreshToken = user.RefreshTokens.FirstOrDefault(rt => rt.Token == command.RefreshToken);

        if (refreshToken is null)
            return;

        refreshToken.ExpiresAt = DateTime.UtcNow;
        refreshToken.RevokedAt = DateTime.UtcNow;
        await _quizContext.SaveChangesAsync().ConfigureAwait(false);
    }
}

