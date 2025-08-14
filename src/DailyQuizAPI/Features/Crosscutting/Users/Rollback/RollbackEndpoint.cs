namespace DailyQuizAPI.Features.Crosscutting.Users.Rollback;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class RollbackEndpoint
{
    private const string ROUTE = "/users/rollback";
    private const string NAME = "RollbackChanges";
    private const string TAG = "Users";
    private const string SUMMARY = "Annule les dernières modifications du compte.";
    private const string DESCRIPTION = "Permet à un utilisateur d’annuler les changements récents (email, pseudo) via un lien sécurisé.";
    private const string OPERATION_ID = "Users_RollbackChanges";
    private const string SUCCESS_DESCRIPTION = "Modifications annulées avec succès.";

    public static void MapRollbackEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] RollbackCommandHandler handler,
                   [FromBody] RollbackCommand command) =>
            {
                await handler.Handle(command).ConfigureAwait(false);
                return Results.Ok();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags(TAG)
        .WithOpenApi(operation =>
        {
            operation.Summary = SUMMARY;
            operation.Description = DESCRIPTION;
            operation.OperationId = OPERATION_ID;
            operation.Responses[StatusCodes.Status200OK.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.NOTFOUND;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}