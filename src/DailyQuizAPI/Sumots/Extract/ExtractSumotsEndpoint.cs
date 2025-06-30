using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Sumots.Extract;

public static class ExtractSumotsEndpoint
{
    private const string SUMOTS_RESOURCE_NAME = "/extractSumots";

    public static void MapExtractSumotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(SUMOTS_RESOURCE_NAME,
            async ([FromServices] ExtractSumotsCommandHandler handler,
                   [FromBody] ExtractSumotsCommand command,
                   CancellationToken cancellationToken) =>
            {
                await handler.Handle(command, cancellationToken).ConfigureAwait(false);
                return Results.Ok();
            })
        .WithName("ExtractSumots")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Sumots")
        .WithOpenApi();
    }
}