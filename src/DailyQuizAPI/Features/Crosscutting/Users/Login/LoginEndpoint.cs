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
    private const string DESCRIPTION = "Retourne un JWT si les identifiants sont valides.";
    private const string OPERATION_ID = "Users_Login";
    private const string SUCCESS_DESCRIPTION = "Connexion réussie.";

    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] LoginCommandHandler handler,
                   [FromBody] LoginCommand request) =>
            {
                var result = await handler.Handle(request).ConfigureAwait(false);
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

