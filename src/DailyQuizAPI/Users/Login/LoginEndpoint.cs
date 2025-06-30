using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Users.Login;

public static class LoginEndpoint
{
    private const string LOGIN_RESOURCE_NAME = "/users/login";

    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(LOGIN_RESOURCE_NAME,
            async ([FromServices] LoginCommandHandler handler,
                   [FromBody] LoginCommand request,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(request, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .WithName("Login")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Users")
        .WithOpenApi();
    }
}
