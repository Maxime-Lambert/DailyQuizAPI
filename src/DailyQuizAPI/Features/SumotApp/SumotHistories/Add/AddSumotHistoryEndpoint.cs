using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.Add;

public static class AddSumotHistoriesEndpoint
{
    private const string ROUTE = "/sumothistories/addrange";
    private const string NAME = "AddSumotHistories";
    private const string TAG = "SumotHistories";
    private const string SUMMARY = "Ajouter un historique de tentative";
    private const string DESCRIPTION = "Enregistre les tentatives d’un utilisateur pour un ou plusieurs mots donnés. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "SumotHistories_Add";
    private const string SUCCESS_DESCRIPTION = "Historiques enregistrés.";

    public static void MapAddSumotHistoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] AddSumotHistoriesCommandHandler handler,
                   [FromBody] AddSumotHistoriesCommand request,
                   ClaimsPrincipal user,
                   CancellationToken ct) =>
            {
                await handler.Handle(request, user, ct).ConfigureAwait(false);
                return Results.Ok();
            })
        .RequireAuthorization(SecurityPolicies.PLAYER)
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
