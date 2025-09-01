namespace DailyQuizAPI.Features.Crosscutting.Users.Export;

using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

public static class ExportUserDataEndpoint
{
    private const string ROUTE = "/users/export";
    private const string NAME = "ExportData";
    private const string TAG = "Users";
    private const string SUMMARY = "Récupère les données d'un utilisateur";
    private const string DESCRIPTION = "Récupère toutes les informations enregistrées d'un utilisateur, ses amis, ses historiques. Requiert une authentification API.";
    private const string OPERATION_ID = "Users_Export";
    private const string SUCCESS_DESCRIPTION = "Informations envoyées.";

    public static void MapExportUserDataEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ExportUserDataCommandHandler handler,
                    ClaimsPrincipal user) =>
            {
                var result = await handler.Handle(user).ConfigureAwait(false);
                return Results.File([.. result.FileContent], result.ContentType, result.FileName);
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
