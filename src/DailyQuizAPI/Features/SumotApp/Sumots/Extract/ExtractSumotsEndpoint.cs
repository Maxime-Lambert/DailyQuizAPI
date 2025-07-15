using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.Sumots.Extract;

public static class ExtractSumotsEndpoint
{
    private const string ROUTE = "/sumots/extract";
    private const string NAME = "ExtractSumots";
    private const string TAG = "Sumots";
    private const string SUMMARY = "Extraire de nouveaux sumots";
    private const string DESCRIPTION = "Parcours une liste de mots pour les ajouter à la base de données de sumots. Requiert une authentification par clé API.";
    private const string OPERATION_ID = "Sumots_Extract";
    private const string SUCCESS_DESCRIPTION = "Mots extraits avec succès.";

    public static void MapExtractSumotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ExtractSumotsCommandHandler handler,
                   [FromBody] ExtractSumotsCommand command,
                   CancellationToken ct) =>
            {
                await handler.Handle(command, ct).ConfigureAwait(false);
                return Results.Ok();
            })
        .RequireAuthorization(SecurityPolicies.SYSTEM)
        .WithName(NAME)
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
