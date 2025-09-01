using DailyQuizAPI.Exceptions;
using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
using DailyQuizAPI.Features.Crosscutting.Users;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;

public class CreateFriendRequestCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(CreateFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var senderId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var user = await _quizContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId, ct)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Connexion invalide");

        if (user.UserName == command.TargetUsername)
            throw new InvalidOperationException("Impossible de s'ajouter soi-même en ami");

        var targetUser = await _quizContext.Users
            .FirstOrDefaultAsync(u => u.UserName == command.TargetUsername, ct)
            .ConfigureAwait(false)
            ?? throw new NotFoundException("L'utilisateur ciblé n'existe pas");

        var exists = await _quizContext.FriendRequests.AnyAsync(fr =>
            fr.RequesterId == senderId && fr.ReceiverId == targetUser.Id ||
            fr.RequesterId == targetUser.Id && fr.ReceiverId == senderId, ct)
            .ConfigureAwait(false);

        if (exists)
            throw new InvalidOperationException("Cette demande d'ami existe déjà");

        var request = new FriendRequest
        {
            RequesterId = senderId,
            ReceiverId = targetUser.Id,
            RequestedAt = DateTime.UtcNow
        };

        _quizContext.FriendRequests.Add(request);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        _cacheService.RemoveByPrefix($"friendRequests:{user.Id}");
        _cacheService.RemoveByPrefix($"friendRequests:{targetUser.Id}");
    }
}
