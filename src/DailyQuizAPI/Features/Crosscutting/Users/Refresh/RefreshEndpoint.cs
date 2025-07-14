using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.Refresh;

public static class RefreshEndpoint
{
    private const string ROUTE = "/users/refresh";
    private const string NAME = "Refresh";
    private const string TAG = "Users";
    private const string SUMMARY = "Rafraîchir un token JWT";
    private const string DESCRIPTION = "Retourne un nouveau JWT à partir d’un refresh token.";
    private const string OPERATION_ID = "Users_Refresh";
    private const string SUCCESS_DESCRIPTION = "Token JWT renvoyé.";

    public static void MapRefreshEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] RefreshCommandHandler handler,
                   [FromBody] RefreshCommand request,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(request, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .WithName(NAME)
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
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
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

