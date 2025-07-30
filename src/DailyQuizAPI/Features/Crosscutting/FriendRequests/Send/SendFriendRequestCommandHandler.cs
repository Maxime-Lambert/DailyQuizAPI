using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;

public class SendFriendRequestCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(SendFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var senderId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (senderId == command.TargetUsername)
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
    }
}
