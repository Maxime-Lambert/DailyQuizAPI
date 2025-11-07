using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddFinish;

public static class AddFinishEndpoint
{
    private const string ROUTE = "/sumotstats/addfinish";
    private const string NAME = "AddFinish";
    private const string TAG = "SumotStats";
    private const string SUMMARY = "Ajouter une fin pour le jour";
    private const string DESCRIPTION = "En fonction du header, ajoute une fin pour le jour fourni en mobile ou web";
    private const string OPERATION_ID = "SumotStats_AddFinish";
    private const string SUCCESS_DESCRIPTION = "Statistiques mises à jour.";

    public static void MapAddFinishEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] AddFinishCommandHandler handler,
                   [FromBody] AddFinishCommand command,
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