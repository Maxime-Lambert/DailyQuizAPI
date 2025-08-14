using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.Login;

public static class LoginEndpoint
{
    private const string ROUTE = "/users/login";
    private const string NAME = "Login";
    private const string TAG = "Users";
    private const string SUMMARY = "Authentifier un utilisateur";
    private const string DESCRIPTION = "Retourne un JWT si les identifiants sont valides et un refreshToken sous forme de Cookie si la requête à un Header X-Client-Type qui vaut SPA sinon, il fait égalemnt partie du body.";
    private const string OPERATION_ID = "Users_Login";
    private const string SUCCESS_DESCRIPTION = "Connexion réussie.";

    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async (HttpRequest request,
                   HttpResponse response,
                   [FromServices] LoginCommandHandler handler,
                   [FromBody] LoginCommand command) =>
            {
                var clientType = request.Headers["X-Client-Type"].ToString().ToUpperInvariant();
                var result = await handler.Handle(command).ConfigureAwait(false);

                if (clientType == "SPA")
                {
                    response.Cookies.Append("refreshToken", result.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    return Results.Ok(new LoginResponse(result.Token, ""));
                }

                return Results.Ok(result);
            })
        .WithName(NAME)
        .Produces<LoginResponse>(StatusCodes.Status200OK)
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

