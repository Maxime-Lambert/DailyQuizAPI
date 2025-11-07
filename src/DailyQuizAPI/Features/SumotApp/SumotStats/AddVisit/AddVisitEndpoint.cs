using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddVisit;

public static class AddVisitEndpoint
{
    private const string ROUTE = "/sumotstats/addvisit";
    private const string NAME = "AddVisit";
    private const string TAG = "SumotStats";
    private const string SUMMARY = "Ajouter une visite pour le jour";
    private const string DESCRIPTION = "En fonction du header, ajoute une visite pour le jour fourni en mobile ou web";
    private const string OPERATION_ID = "SumotStats_AddVisit";
    private const string SUCCESS_DESCRIPTION = "Statistiques mises à jour.";

    public static void MapAddVisitEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] AddVisitCommandHandler handler,
                   [FromBody] AddVisitCommand command,
                   CancellationToken ct) =>
            {
                await handler.Handle(command, ct).ConfigureAwait(false);
                return Results.NoContent();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status204NoContent)
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