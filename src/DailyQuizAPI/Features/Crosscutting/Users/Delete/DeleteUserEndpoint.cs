namespace DailyQuizAPI.Features.Crosscutting.Users.Delete;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

public static class DeleteUserEndpoint
{
    private const string ROUTE = "/users";
    private const string NAME = "DeleteUser";
    private const string TAG = "Users";
    private const string SUMMARY = "Supprime un utilisateur";
    private const string DESCRIPTION = "Supprime un utilisateur et toutes ses informations. Requiert une authentification API.";
    private const string OPERATION_ID = "Users_Delete";
    private const string SUCCESS_DESCRIPTION = "Utilisateur supprimé.";

    public static void MapDeleteUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ROUTE,
            async ([FromServices] DeleteUserCommandHandler handler,
                    [FromBody] DeleteUserCommand command,
                   ClaimsPrincipal claims) =>
            {
                await handler.Handle(command, claims).ConfigureAwait(false);
                return Results.NoContent();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status204NoContent)
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
            operation.Responses[StatusCodes.Status204NoContent.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.UNAUTHORIZED;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}
