using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Sumots.GetSumots;

public static class GetSumotsEndpoint
{
    private const string SumotsResourceName = "/sumots";

    public static void MapGetSumotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(SumotsResourceName,
            async ([FromServices] GetSumotsQueryHandler handler,
                   [AsParameters] GetSumotsQuery query,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(query, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .WithName("GetSumots")
        .Produces<IEnumerable<GetSumotsResponse>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .WithTags("Sumots")
        .WithOpenApi();
    }

}
