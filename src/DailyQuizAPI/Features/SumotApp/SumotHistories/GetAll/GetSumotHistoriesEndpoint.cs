using DailyQuizAPI.Middlewares;
using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace DailyQuizAPI.Features.SumotApp.SumotHistories.GetAll;

public static class GetSumotHistoriesEndpoint
{
    private const string ROUTE = "/sumothistories";
    private const string NAME = "GetSumotHistories";
    private const string TAG = "SumotHistories";
    private const string SUMMARY = "Récupérer les historiques";
    private const string DESCRIPTION = "Retourne l’historique des tentatives des amis pour les mots joués. Requiert un joueur authentifié.";
    private const string OPERATION_ID = "SumotHistories_Get";
    private const string SUCCESS_DESCRIPTION = "Historique récupéré.";

    public static void MapGetSumotHistoriesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet(ROUTE,
            async ([FromServices] GetSumotHistoriesQueryHandler handler,
                   [AsParameters] GetSumotHistoriesQuery request,
                   ClaimsPrincipal user,
                   CancellationToken ct) =>
            {
                var result = await handler.Handle(request, user, ct).ConfigureAwait(false);
                return Results.Ok(result);
            })
        .RequireAuthorization(SecurityPolicies.PLAYER)
        .WithName(NAME)
        .Produces<List<GetSumotHistoriesResponse>>(StatusCodes.Status200OK)
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

