using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.Crosscutting.Users.GetOne;

public static class GetUserEndpoint
{
    private const string ROUTE = "/users/{id}";
    private const string NAME = "GetUser";
    private const string TAG = "Users";
    private const string SUMMARY = "Récupérer un utilisateur";
    private const string DESCRIPTION = "Permet de récupérer le pseudo, l'email, si l'email est confirmé, le mode daltonien, le type de clavier et l'id d'un utilisateur.";
    private const string OPERATION_ID = "Users_Get";
    private const string SUCCESS_DESCRIPTION = "Informations acquises.";

    public static void MapGetUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE, async (
            [FromServices] GetUserQueryHandler handler,
            ClaimsPrincipal claims) =>
        {
            var result = await handler.Handle(claims).ConfigureAwait(false);
            return Results.Ok(result);
        })
        .WithName(NAME)
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .Produces(StatusCodes.Status200OK)
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
            operation.Responses[StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.NOTFOUND;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}

