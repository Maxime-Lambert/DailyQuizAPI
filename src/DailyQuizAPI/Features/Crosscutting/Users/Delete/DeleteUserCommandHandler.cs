using DailyQuizAPI.Persistence;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.Users.Delete;

public sealed class DeleteUserCommandHandler(UserManager<User> userManager, QuizContext quizContext)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(DeleteUserCommand command, ClaimsPrincipal claims)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

        var checkPassword = await _userManager.CheckPasswordAsync(user, command.Password).ConfigureAwait(false);
        if (!checkPassword)
        {
            throw new InvalidOperationException("Mot de passe incorrect");
        }

        await _userManager.DeleteAsync(user).ConfigureAwait(false);

        var friends = _quizContext.FriendRequests
            .Where(fr => fr.ReceiverId == userId || fr.RequesterId == userId);
        var histories = _quizContext.SumotHistories
            .Where(h => h.UserId == userId);

        _quizContext.FriendRequests.RemoveRange(friends);
        _quizContext.SumotHistories.RemoveRange(histories);
        await _quizContext.SaveChangesAsync().ConfigureAwait(false);
    }
}