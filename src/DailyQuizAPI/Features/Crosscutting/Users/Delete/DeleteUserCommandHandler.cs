using DailyQuizAPI.Common.Exceptions;
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
            ?? throw new NotFoundException("Utilisateur introuvable dans les revendications.");

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("L'utilisateur connecté est introuvable.");

        var checkPassword = await _userManager.CheckPasswordAsync(user, command.Password).ConfigureAwait(false);

        if (!checkPassword)
            throw new InvalidOperationException("Mot de passe incorrect.");

        var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

        var friends = _quizContext.FriendRequests
            .Where(fr => fr.ReceiverId == userId || fr.RequesterId == userId);
        var histories = _quizContext.SumotHistories
            .Where(h => h.UserId == userId);

        _quizContext.FriendRequests.RemoveRange(friends);
        _quizContext.SumotHistories.RemoveRange(histories);
        await _quizContext.SaveChangesAsync().ConfigureAwait(false);
    }
}