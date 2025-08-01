using DailyQuizAPI.Features.Crosscutting.FriendRequests.Create;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.FriendRequests.Send;

public static class CreateFriendRequestEndpoint
{
    private const string ROUTE = "/friendrequests/{targetUsername}";
    private const string NAME = "SendFriendRequest";
    private const string TAG = "FriendRequests";
    private const string SUMMARY = "Envoyer une demande d’ami";
    private const string DESCRIPTION = "Crée une nouvelle demande d’ami. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "FriendRequests_Send";
    private const string SUCCESS_DESCRIPTION = "Demande envoyée avec succès.";

    public static void MapSendFriendRequestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE, async (
            [FromServices] CreateFriendRequestCommandHandler handler,
            [FromBody] CreateFriendRequestCommand command,
            ClaimsPrincipal currentUser,
            CancellationToken ct) =>
        {
            await handler.Handle(command, currentUser, ct).ConfigureAwait(false);
            return Results.Created();
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
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


