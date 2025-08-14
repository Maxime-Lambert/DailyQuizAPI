using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.ResendConfirmation;

public static class ResendConfirmationEndpoint
{
    private const string ROUTE = "/users/resendConfirmation";
    private const string NAME = "ResendConfirmation";
    private const string TAG = "Users";
    private const string SUMMARY = "Envoie un nouveau mail de confirmation";
    private const string DESCRIPTION = "Envoie un nouveau mail de confirmation à l'adresse spécifiée";
    private const string OPERATION_ID = "Users_ResendConfirmation";
    private const string SUCCESS_DESCRIPTION = "Mail envoyé.";

    public static void MapCreateUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ResendConfirmationCommandHandler handler,
                   [FromBody] ResendConfirmationCommand request) =>
            {
                await handler.Handle(request).ConfigureAwait(false);
                return Results.Ok();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
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
            operation.Responses[StatusCodes.Status404NotFound.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.NOTFOUND;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}
