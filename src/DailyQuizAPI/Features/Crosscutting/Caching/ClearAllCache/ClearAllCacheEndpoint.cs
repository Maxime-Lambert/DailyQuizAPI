namespace DailyQuizAPI.Features.Crosscutting.Caching.ClearAllCache;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class ClearAllCacheEndpoint
{
    private const string ROUTE = "/cache/clearall";
    private const string NAME = "ClearAllCache";
    private const string TAG = "Cache";
    private const string SUMMARY = "Supprime le cache";
    private const string DESCRIPTION = "Supprime le cache. Utile si une modification à la main a été apportée.";
    private const string OPERATION_ID = "Cache_Clear";
    private const string SUCCESS_DESCRIPTION = "La cache a été supprimé avec succès.";

    public static void MapClearAllCacheEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE, async (
            [FromServices] ClearAllCacheCommandHandler handler) =>
        {
            await handler.Handle().ConfigureAwait(false);
            return Results.Ok();
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

