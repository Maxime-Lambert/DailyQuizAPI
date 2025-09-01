using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.GetAll;

public static class GetFriendRequestsEndpoint
{
    private const string ROUTE = "/friendrequests";
    private const string NAME = "GetFriendRequests";
    private const string TAG = "FriendRequests";
    private const string SUMMARY = "Récupérer les demandes d’ami";
    private const string DESCRIPTION = "Retourne les demandes envoyées, reçues et acceptées. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "FriendRequests_Get";
    private const string SUCCESS_DESCRIPTION = "Liste des amis et demandes d'amis récupérées.";

    public static void MapGetFriendRequestsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE, async (
            [FromServices] GetFriendRequestsQueryHandler handler,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(user, ct).ConfigureAwait(false);
            return Results.Ok(result);
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .Produces(StatusCodes.Status200OK)
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
            operation.Responses[StatusCodes.Status200OK.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status401Unauthorized.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.UNAUTHORIZED;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

