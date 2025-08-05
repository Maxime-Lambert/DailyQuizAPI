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
            async (HttpRequest request,
                   HttpResponse response,
                   [FromBody] RefreshCommand command,
                   [FromServices] RefreshCommandHandler handler,
                   CancellationToken ct) =>
            {
                if (request.Cookies.TryGetValue("refreshToken", out var tokenFromCookie))
                {
                    if (string.IsNullOrWhiteSpace(tokenFromCookie))
                        return Results.BadRequest("Refresh token cookie is empty.");

                    var resultFromCookie = await handler.Handle(
                        new RefreshCommand(tokenFromCookie),
                        ct
                    ).ConfigureAwait(false);
                    response.Cookies.Append("refreshToken", resultFromCookie.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                    return Results.Ok(new { accessToken = resultFromCookie.Token });
                }
                var result = await handler.Handle(command, ct).ConfigureAwait(false);
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

