using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyQuizAPI.Features.FriendRequests.Send;

public static class SendFriendRequestEndpoint
{
    private const string FRIENDREQUEST_SEND_ENDPOINT_ROUTE = "/friendrequests/send";

    public static void MapSendFriendRequestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(FRIENDREQUEST_SEND_ENDPOINT_ROUTE, async (
            [FromServices] SendFriendRequestCommandHandler handler,
            [FromBody] AcceptFriendRequestCommand command,
            ClaimsPrincipal currentUser,
            CancellationToken ct) =>
                {
                    await handler.Handle(command, currentUser, ct).ConfigureAwait(false);
                    return Results.Created();
                })
        .RequireAuthorization()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .WithName("Send")
        .WithTags("FriendRequests")
        .WithOpenApi();
    }
}

