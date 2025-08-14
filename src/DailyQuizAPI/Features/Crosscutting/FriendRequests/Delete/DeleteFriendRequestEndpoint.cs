using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Delete;

public static class DeleteFriendRequestEndpoint
{
    private const string ROUTE = "/friendrequests/{targetId}";
    private const string NAME = "DeleteFriendRequest";
    private const string TAG = "FriendRequests";
    private const string SUMMARY = "Refuser une demande d’ami";
    private const string DESCRIPTION = "Supprime une demande d’ami en attente. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "FriendRequests_Delete";
    private const string SUCCESS_DESCRIPTION = "Demande supprimée avec succès.";

    public static void MapDeleteFriendRequestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ROUTE, async (
            [FromServices] DeleteFriendRequestCommandHandler handler,
            [FromRoute] string targetId,
            ClaimsPrincipal currentUser,
            CancellationToken ct) =>
        {
            await handler.Handle(targetId, currentUser, ct).ConfigureAwait(false);
            return Results.NoContent();
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
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
            operation.Responses[StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.UNAUTHORIZED;
            operation.Responses[StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.NOTFOUND;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

