using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Delete;

public sealed class DeleteFriendRequestCommandHandler(QuizContext quizContext, ICacheService cacheService)
{
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task Handle(DeleteFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var friendId = command.FriendId;

        var request = await _quizContext.FriendRequests
            .FirstOrDefaultAsync(fr =>
                (fr.RequesterId == userId && fr.ReceiverId == friendId ||
                 fr.RequesterId == friendId && fr.ReceiverId == userId),
                ct)
            .ConfigureAwait(false) ?? throw new InvalidOperationException("Friendship not found.");

        _quizContext.FriendRequests.Remove(request);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        _cacheService.Remove($"friendRequests:{userId}");
        _cacheService.Remove($"friendRequests:{command.FriendId}");
    }
}
