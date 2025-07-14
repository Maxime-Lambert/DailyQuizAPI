using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Accept;

public static class AcceptFriendRequestEndpoint
{
    private const string ROUTE = "/friendrequests/accept";
    private const string NAME = "AcceptFriendRequest";
    private const string TAG = "FriendRequests";
    private const string SUMMARY = "Accepter une demande d’ami";
    private const string DESCRIPTION = "Accepte une demande d’ami existante. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "FriendRequests_Accept";
    private const string SUCCESS_DESCRIPTION = "Demande acceptée avec succès.";

    public static void MapAcceptFriendRequestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE, async (
            [FromServices] AcceptFriendRequestCommandHandler handler,
            [FromBody] AcceptFriendRequestCommand command,
            ClaimsPrincipal currentUser,
            CancellationToken ct) =>
        {
            await handler.Handle(command, currentUser, ct).ConfigureAwait(false);
            return Results.Ok();
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

