namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotUsername;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class ForgotUsernameEndpoint
{
    private const string ROUTE = "/users/forgotusername";
    private const string NAME = "ForgotUserName";
    private const string TAG = "Users";
    private const string SUMMARY = "Pour récupérer un nom d'utilisateur";
    private const string DESCRIPTION = "Envoie un mail avec le nom d'utilsateur lié à l'email.";
    private const string OPERATION_ID = "Users_ForgotUserName";
    private const string SUCCESS_DESCRIPTION = "Mail envoyé.";

    public static void MapForgotUsernameEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ForgotUsernameCommandHandler handler,
                    [FromBody] ForgotUsernameCommand command) =>
            {
                await handler.Handle(command).ConfigureAwait(false);
                return Results.NoContent();
            })
        .WithName(NAME)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status429TooManyRequests)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithTags(TAG)
        .WithOpenApi(operation =>
        {
            operation.Summary = SUMMARY;
            operation.Description = DESCRIPTION;
            operation.OperationId = OPERATION_ID;
            operation.Responses[StatusCodes.Status204NoContent.ToString(CultureInfo.InvariantCulture)].Description = SUCCESS_DESCRIPTION;
            operation.Responses[StatusCodes.Status429TooManyRequests.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.TOOMANYREQUESTS;
            operation.Responses[StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture)].Description = SwaggerErrorDescriptions.SERVERERROR;
            return operation;
        });
    }
}
