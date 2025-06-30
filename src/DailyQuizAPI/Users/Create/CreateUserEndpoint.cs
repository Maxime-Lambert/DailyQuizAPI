using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.Users.Create;

public static class CreateUserEndpoint
{
    private const string USERS_RESOURCE_NAME = "/users";

    public static void MapCreateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(USERS_RESOURCE_NAME,
            async ([FromServices] CreateUserCommandHandler handler,
                   [FromBody] CreateUserCommand request) =>
            {
                await handler.Handle(request).ConfigureAwait(false);
                return Results.Created();
            })
        .WithName("CreateUser")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("Users")
        .WithOpenApi();
    }
}