using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.AppSettings.Create;

public static class CreateAppSettingEndpoint
{
    private const string ROUTE = "/appsettings";
    private const string NAME = "CreateAppSetting";
    private const string TAG = "AppSettings";
    private const string SUMMARY = "Créer un nouvel appsetting";
    private const string DESCRIPTION = "Enregistre un appsetting avec clé / valeur.";
    private const string OPERATION_ID = "AppSettings_Create";
    private const string SUCCESS_DESCRIPTION = "AppSetting créé avec succès.";

    public static void MapPostAppSettingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromBody] CreateAppSettingCommand command,
            [FromServices] CreateAppSettingCommandHandler handler,
            CancellationToken ct) =>
            {
                await handler.Handle(command, ct).ConfigureAwait(false);
                return Results.Created();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags(TAG)
        .WithOpenApi(operation =>
        {
            operation.Summary = SUMMARY;
            operation.Description = DESCRIPTION;
            operation.OperationId = OPERATION_ID;
            operation.Responses[StatusCodes.Status201Created.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.BADREQUEST;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}