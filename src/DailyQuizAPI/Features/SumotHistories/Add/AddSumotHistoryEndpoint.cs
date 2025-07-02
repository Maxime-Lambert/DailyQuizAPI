using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotHistories.Add;

public static class AddSumotHistoryEndpoint
{
    private const string SUMOTS_RESOURCE_NAME = "/sumothistories";

    public static void MapAddSumotHistoryEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(SUMOTS_RESOURCE_NAME,
            async ([FromServices] AddSumotHistoryCommandHandler handler,
                   [FromBody] AddSumotHistoryCommand request,
                   ClaimsPrincipal user,
                   CancellationToken ct) =>
            {
                await handler.Handle(request, user, ct).ConfigureAwait(false);
                return Results.Ok();
            })
        .RequireAuthorization()
        .WithName("AddSumotHistory")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("SumotHistories")
        .WithOpenApi();
    }
}