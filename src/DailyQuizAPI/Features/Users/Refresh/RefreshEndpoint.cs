using DailyQuizAPI.Features.Users.Login;
using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Features.Users.Refresh;

public static class RefreshEndpoint
{
    private const string REFRESH_TOKEN_RESOURCE_NAME = "/users/refresh";

    public static void MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(REFRESH_TOKEN_RESOURCE_NAME,
            async ([FromServices] RefreshCommandHandler handler,
                   [FromBody] RefreshCommand request,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(request, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .WithName("Refresh")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Users")
        .WithOpenApi();
    }
}
