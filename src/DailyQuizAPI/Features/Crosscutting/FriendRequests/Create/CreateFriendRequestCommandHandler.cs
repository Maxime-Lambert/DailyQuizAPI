using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
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
        var senderId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await _quizContext.Users
            .FirstOrDefaultAsync(u => u.Id == senderId, ct)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        if (user.UserName == command.TargetUsername)
            throw new InvalidOperationException("You cannot friend yourself.");

        var targetUser = await _quizContext.Users
            .FirstOrDefaultAsync(u => u.UserName == command.TargetUsername, ct)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("Ce nom d'utilisateur n'existe pas.");

        var exists = await _quizContext.FriendRequests.AnyAsync(fr =>
            fr.RequesterId == senderId && fr.ReceiverId == targetUser.Id ||
            fr.RequesterId == targetUser.Id && fr.ReceiverId == senderId, ct)
            .ConfigureAwait(false);

        if (exists)
            throw new InvalidOperationException("Friend request already exists.");

        var request = new FriendRequest
        {
            RequesterId = senderId,
            ReceiverId = targetUser.Id,
            RequestedAt = DateTime.UtcNow
        };

        _quizContext.FriendRequests.Add(request);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);

        _cacheService.Remove($"friendRequests:{user.Id}");
        _cacheService.Remove($"friendRequests:{targetUser.Id}");
    }
}
