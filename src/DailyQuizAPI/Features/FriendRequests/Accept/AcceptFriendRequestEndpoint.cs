using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyQuizAPI.Features.FriendRequests.Accept;

public static class AcceptFriendRequestEndpoint
{
    private const string FRIENDREQUEST_ACCEPT_ENDPOINT_ROUTE = "/friendrequests/accept";

    public static void MapAcceptFriendRequestEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(FRIENDREQUEST_ACCEPT_ENDPOINT_ROUTE, async (
            [FromServices] AcceptFriendRequestCommandHandler handler,
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
        .WithName("Accept")
        .WithTags("FriendRequests")
        .WithOpenApi();
    }
}

