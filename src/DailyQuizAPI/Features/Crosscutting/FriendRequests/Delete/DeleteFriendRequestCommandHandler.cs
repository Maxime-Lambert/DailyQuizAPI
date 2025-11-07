using DailyQuizAPI.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Delete;

public sealed class DeleteFriendRequestCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(string targetId, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");
        var friendId = targetId;

        var request = await _quizContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                (fr.RequesterId == userId && fr.ReceiverId == friendId ||
                 fr.RequesterId == friendId && fr.ReceiverId == userId),
                ct)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("L'utilisateur ciblé n'existe pas");

        _quizContext.FriendRequests.Remove(request);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        _cacheService.RemoveByPrefix($"friendRequests:{userId}");
        _cacheService.RemoveByPrefix($"friendRequests:{targetId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{userId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{targetId}");
    }
}
