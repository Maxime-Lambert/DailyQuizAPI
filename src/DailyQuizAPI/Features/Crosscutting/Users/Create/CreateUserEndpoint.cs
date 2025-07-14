using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.Create;

public static class CreateUserEndpoint
{
    private const string ROUTE = "/users";
    private const string NAME = "CreateUser";
    private const string TAG = "Users";
    private const string SUMMARY = "Créer un nouvel utilisateur";
    private const string DESCRIPTION = "Crée un utilisateur avec nom d’utilisateur, email et mot de passe. Requiert une authentification API.";
    private const string OPERATION_ID = "Users_Create";
    private const string SUCCESS_DESCRIPTION = "Utilisateur créé.";

    public static void MapCreateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] CreateUserCommandHandler handler,
                   [FromBody] CreateUserCommand request) =>
            {
                await handler.Handle(request).ConfigureAwait(false);
                return Results.Created();
            })
        .RequireAuthorization(SecurityPolicies.SYSTEM)
        .WithName(NAME)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags(TAG)
        .WithOpenApi(operation =>
        {
            operation.Summary = SUMMARY;
            operation.Description = DESCRIPTION;
            operation.OperationId = OPERATION_ID;
            operation.Responses[StatusCodes.Status201Created.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.UNAUTHORIZED;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}
