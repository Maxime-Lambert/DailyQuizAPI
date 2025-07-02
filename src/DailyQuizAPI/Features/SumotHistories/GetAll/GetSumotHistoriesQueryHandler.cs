using DailyQuizAPI.Features.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotHistories.GetAll;

public class GetSumotHistoriesQueryHandler(UserManager<User> userManager)
{
    private readonly UserManager<User> _userManager = userManager;

    public async Task<List<GetSumotHistoriesResponse>> Handle(GetSumotHistoriesQuery query, ClaimsPrincipal claims, CancellationToken ct)
    {
        var userId = claims.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var user = await _userManager.FindByIdAsync(userId).ConfigureAwait(false)
            ?? throw new InvalidOperationException("User not found.");

        return [.. user.SumotHistories
            .OrderByDescending(h => h.Day)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(h => new GetSumotHistoriesResponse(
                h.Id, h.Word, h.Tries, h.Day, h.Ranking
            ))];
    }
}
