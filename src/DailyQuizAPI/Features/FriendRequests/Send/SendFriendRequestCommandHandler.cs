using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.FriendRequests.Send;

public class SendFriendRequestCommandHandler(QuizContext quizContext)
{
    private readonly QuizContext _quizContext = quizContext;

    public async Task Handle(AcceptFriendRequestCommand command, ClaimsPrincipal claims, CancellationToken ct)
    {
        var senderId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (senderId == command.TargetUserId)
            throw new InvalidOperationException("You cannot friend yourself.");

        var exists = await _quizContext.FriendRequests.AnyAsync(fr =>
            fr.RequesterId == senderId && fr.ReceiverId == command.TargetUserId ||
            fr.RequesterId == command.TargetUserId && fr.ReceiverId == senderId, ct)
            .ConfigureAwait(false);

        if (exists)
            throw new InvalidOperationException("Friend request already exists.");

        var request = new FriendRequest
        {
            RequesterId = senderId,
            ReceiverId = command.TargetUserId
        };

        _quizContext.FriendRequests.Add(request);

        await _quizContext.SaveChangesAsync(ct).ConfigureAwait(false);
    }
}
