using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.RemoveFriend;

public static class RemoveFriendEndpoint
{
    private const string ROUTE = "/friendrequests/removefriend";
    private const string NAME = "RemoveFriend";
    private const string TAG = "FriendRequests";
    private const string SUMMARY = "Retirer un ami";
    private const string DESCRIPTION = "Supprime un ami existant de votre liste. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "FriendRequests_RemoveFriend";
    private const string SUCCESS_DESCRIPTION = "Ami supprimé avec succès.";

    public static void MapRemoveFriendEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE, async (
            [FromServices] RemoveFriendCommandHandler handler,
            [FromBody] RemoveFriendCommand command,
            ClaimsPrincipal currentUser,
            CancellationToken ct) =>
        {
            await handler.HandleAsync(command, currentUser, ct).ConfigureAwait(false);
            return Results.Ok();
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
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


