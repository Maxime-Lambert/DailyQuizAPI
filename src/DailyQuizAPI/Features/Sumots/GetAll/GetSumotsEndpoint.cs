using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Features.Sumots.GetAll;

public static class GetSumotsEndpoint
{
    private const string SUMOTS_RESOURCE_NAME = "/sumots";

    public static void MapGetSumotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(SUMOTS_RESOURCE_NAME,
            async ([FromServices] GetSumotsQueryHandler handler,
                   [AsParameters] GetSumotsQuery query,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(query, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .WithName("GetSumots")
        .Produces<GetSumotsResponseList>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Sumots")
        .WithOpenApi();
    }
}
