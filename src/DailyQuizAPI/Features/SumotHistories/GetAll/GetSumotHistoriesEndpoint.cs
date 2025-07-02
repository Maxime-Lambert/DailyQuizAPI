using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotHistories.GetAll;

public static class GetSumotHistoriesEndpoint
{
    private const string SUMOTS_RESOURCE_NAME = "/sumothistories";

    public static void MapGetSumotHistoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(SUMOTS_RESOURCE_NAME,
            async ([FromServices] GetSumotHistoriesQueryHandler handler,
                   [AsParameters] GetSumotHistoriesQuery request,
                   ClaimsPrincipal user,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(request, user, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .RequireAuthorization()
        .WithName("GetSumotHistories")
        .Produces<List<GetSumotHistoriesResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("SumotHistories")
        .WithOpenApi();
    }
}
