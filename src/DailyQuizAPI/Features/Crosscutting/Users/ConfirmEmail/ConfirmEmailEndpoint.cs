using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace DailyQuizAPI.Features.Crosscutting.Users.ConfirmEmail;

public static class ConfirmEmailEndpoint
{
    private const string ROUTE = "/users/confirmemail";
    private const string NAME = "ConfirmEmail";
    private const string TAG = "Users";
    private const string SUMMARY = "Confirme l’adresse e-mail d’un utilisateur.";
    private const string DESCRIPTION = "Confirme l’adresse e-mail d’un utilisateur à l’aide du lien reçu par mail.";
    private const string OPERATION_ID = "Users_ConfirmEmail";
    private const string SUCCESS_DESCRIPTION = "E-mail confirmé avec succès.";

    public static void MapConfirmEmailEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ConfirmEmailCommandHandler handler,
                   [FromBody] ConfirmEmailCommand command) =>
            {
                await handler.Handle(command).ConfigureAwait(false);
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

