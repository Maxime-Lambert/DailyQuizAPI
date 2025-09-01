using DailyQuizAPI.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;

public class AcceptFriendRequestCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(AcceptFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var userFriends = _quizContext.FriendRequests.Where(fr => fr.RequesterId == userId || fr.ReceiverId == userId);
        var userFriendCount = await userFriends.CountAsync(ct).ConfigureAwait(false);
        if (userFriendCount == 20)
            throw new InvalidOperationException("Le nombre d'amis est limité à 20 pour le moment");

        var friendRequest = await _quizContext.FriendRequests.FirstOrDefaultAsync(fr =>
                fr.RequesterId == command.TargetUserId && fr.ReceiverId == userId,
            ct).ConfigureAwait(false)
            ?? throw new NotFoundException("L'utilisateur ciblé n'existe pas");

        friendRequest.IsAccepted = true;
        friendRequest.AcceptedAt = DateTime.UtcNow;

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        _cacheService.RemoveByPrefix($"friendRequests:{userId}");
        _cacheService.RemoveByPrefix($"friendRequests:{command.TargetUserId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{userId}");
        _cacheService.RemoveByPrefix($"sumotHistories:{command.TargetUserId}");
    }
}
