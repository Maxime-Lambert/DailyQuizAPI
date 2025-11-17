using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.GetSumotStats;

public static class GetSumotStatsEndpoint
{
    private const string ROUTE = "/sumotstats";
    private const string NAME = "GetStats";
    private const string TAG = "SumotStats";
    private const string SUMMARY = "Récupère toutes les statistiques";
    private const string DESCRIPTION = "Récupère toutes les statistiques pour l'application Sumot";
    private const string OPERATION_ID = "SumotStats_GetAll";
    private const string SUCCESS_DESCRIPTION = "Statistiques récupérées.";

    public static void MapGetSumotStatsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE,
            async ([FromServices] GetSumotStatsQueryHandler handler,
                   CancellationToken ct) =>
            {
                await handler.Handle(ct).ConfigureAwait(false);
                return Results.NoContent();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status200OK)
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
