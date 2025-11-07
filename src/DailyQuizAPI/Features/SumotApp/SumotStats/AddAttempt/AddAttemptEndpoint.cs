using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.SumotApp.SumotStats.AddAttempt;

public static class AddAttemptEndpoint
{
    private const string ROUTE = "/sumotstats/addattempt";
    private const string NAME = "AddAttempt";
    private const string TAG = "SumotStats";
    private const string SUMMARY = "Ajouter un essai pour le jour";
    private const string DESCRIPTION = "En fonction du header, ajoute un essai pour le jour fourni en mobile ou web";
    private const string OPERATION_ID = "SumotStats_AddAttempt";
    private const string SUCCESS_DESCRIPTION = "Statistiques mises à jour.";

    public static void MapAddAttemptEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] AddAttemptCommandHandler handler,
                   [FromBody] AddAttemptCommand command,
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