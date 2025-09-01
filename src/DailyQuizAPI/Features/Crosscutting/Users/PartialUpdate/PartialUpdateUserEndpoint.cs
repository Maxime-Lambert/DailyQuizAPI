using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.Users.PartialUpdate;

public static class PartialUpdateUserEndpoint
{
    private const string ROUTE = "/users";
    private const string NAME = "PartialUpdateUser";
    private const string TAG = "Users";
    private const string SUMMARY = "Mettre à jour partiellement un utilisateur";
    private const string DESCRIPTION = "Permet de modifier le mot de passe, le pseudo, l'email, le mode daltonien ou les paramètres de clavier.";
    private const string OPERATION_ID = "Users_PartialUpdate";
    private const string SUCCESS_DESCRIPTION = "Mise à jour effectuée.";

    public static void MapPartialUpdateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch(ROUTE, async (
            [FromBody] PartialUpdateUserCommand command,
            [FromServices] PartialUpdateUserCommandHandler handler,
            ClaimsPrincipal claims) =>
        {
            await handler.Handle(command, claims).ConfigureAwait(false);
            return Results.NoContent();
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .Produces(StatusCodes.Status204NoContent)
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
            operation.Responses[StatusCodes.Status204NoContent.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.NOTFOUND;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

