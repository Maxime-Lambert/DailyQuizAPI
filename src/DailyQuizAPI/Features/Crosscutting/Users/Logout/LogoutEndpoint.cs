using DailyQuizAPI.Features.Crosscutting.Users.Login;
using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.Logout;

public static class LogoutEndpoint
{
    private const string ROUTE = "/users/logout";
    private const string NAME = "Logout";
    private const string TAG = "Users";
    private const string SUMMARY = "Déconnecter un utilisateur";
    private const string DESCRIPTION = "Invalide le refreshtoken et écrase le cookie si un Header X-Client-Type qui vaut SPA est présent.";
    private const string OPERATION_ID = "Users_Logout";
    private const string SUCCESS_DESCRIPTION = "Déconnexion réussie.";

    public static void MapLogoutEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async (HttpRequest request,
                   HttpResponse response,
                   [FromServices] LogoutCommandHandler handler,
                   [FromBody] LogoutCommand? command) =>
            {
                var clientType = request.Headers["X-Client-Type"].ToString().ToUpperInvariant();

                if (clientType == "SPA")
                {
                    var refreshToken = request.Cookies["refreshToken"];
                    if (refreshToken is null)
                    {
                        return Results.NoContent();
                    }
                    var commandSpa = new LogoutCommand(refreshToken);
                    response.Cookies.Append("refreshToken", "", new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(-1)
                    });
                    await handler.Handle(commandSpa).ConfigureAwait(false);
                    return Results.NoContent();
                }
                else
                {
                    await handler.Handle(command!).ConfigureAwait(false);
                    return Results.NoContent();
                }
            })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .Produces<LoginResponse>(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
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
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

