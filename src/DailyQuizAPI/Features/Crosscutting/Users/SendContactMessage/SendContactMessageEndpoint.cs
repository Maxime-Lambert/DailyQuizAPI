namespace DailyQuizAPI.Features.Crosscutting.Users.SendContactMessage;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class SendContactMessageEndpoint
{
    private const string ROUTE = "/users/sendmail";
    private const string NAME = "SendContactMessage";
    private const string TAG = "Users";
    private const string SUMMARY = "Envoie un mail de contact.";
    private const string DESCRIPTION = "Permet à un utilisateur d’envoyer un mail sur l'adresse de contact.";
    private const string OPERATION_ID = "Users_SendContactMessage";
    private const string SUCCESS_DESCRIPTION = "Message envoyé avec succès.";

    public static void MapRollbackEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] SendContactMessageCommandHandler handler,
                   [FromBody] SendContactMessageCommand command) =>
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