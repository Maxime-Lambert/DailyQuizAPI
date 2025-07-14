using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.Sumots.GetAll;

public static class GetSumotsEndpoint
{
    private const string ROUTE = "/sumots";
    private const string NAME = "GetSumots";
    private const string TAG = "Sumots";
    private const string SUMMARY = "Récupérer les mots (sumots)";
    private const string DESCRIPTION = "Retourne les mots disponibles depuis une version donnée ou une date. Requiert une authentification par clé API.";
    private const string OPERATION_ID = "Sumots_Get";
    private const string SUCCESS_DESCRIPTION = "Liste des mots récupérée.";

    public static void MapGetSumotsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE,
            async ([FromServices] GetSumotsQueryHandler handler,
                   [AsParameters] GetSumotsQuery query,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(query, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .RequireAuthorization(SecurityPolicies.SYSTEM)
        .WithName(NAME)
        .Produces<GetSumotsResponseList>(StatusCodes.Status200OK)
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

