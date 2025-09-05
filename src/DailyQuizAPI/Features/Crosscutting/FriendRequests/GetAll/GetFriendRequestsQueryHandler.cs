using DailyQuizAPI.Features.Crosscutting.Caching;
using DailyQuizAPI.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;

public sealed class GetFriendRequestsQueryHandler(QuizContext quizContext, ICacheService cacheService)
{
    private const string SENT_TEXT = "Sent";
    private const string RECEIVED_TEXT = "Received";
    private readonly QuizContext _quizContext = quizContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GetFriendRequestsResponse> Handle(ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Connexion invalide");

        var cacheKey = $"friendRequests:{userId}";

        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            var requests = await _quizContext.FriendRequests
                .Include(fr => fr.Requester)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.RequesterId == userId || fr.ReceiverId == userId)
                .ToListAsync(ct)
                .ConfigureAwait(false);

            var accepted = requests
                .Where(fr => fr.IsAccepted)
                .Select(fr =>
                {
                    var other = fr.RequesterId == userId ? fr.Receiver : fr.Requester;
                    var direction = fr.RequesterId == userId ? SENT_TEXT : RECEIVED_TEXT;
                    return new FriendRequestResponse(fr.Id, direction, other.Id, other.UserName!, other.Email!);
                })
                .ToList();

            var pending = requests
                .Where(fr => !fr.IsAccepted)
                .Select(fr =>
                {
                    var other = fr.RequesterId == userId ? fr.Receiver : fr.Requester;
                    var direction = fr.RequesterId == userId ? SENT_TEXT : RECEIVED_TEXT;
                    return new FriendRequestResponse(fr.Id, direction, other.Id, other.UserName!, other.Email!);
                })
                .ToList();

            return new GetFriendRequestsResponse(
                new(accepted),
                new(pending)
            );
        }, TimeSpan.FromMinutes(60)).ConfigureAwait(false);
    }
}

