namespace DailyQuizAPI.Features.Crosscutting.Users.ForgotPassword;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class ForgotPasswordEndpoint
{
    private const string ROUTE = "/users/forgotpassword";
    private const string NAME = "ForgotPassword";
    private const string TAG = "Users";
    private const string SUMMARY = "Pour récupérer un mot de passe";
    private const string DESCRIPTION = "Envoie un lien qui permet de mettre à jour le mot de passe.";
    private const string OPERATION_ID = "Users_ForgotPassword";
    private const string SUCCESS_DESCRIPTION = "Mail envoyé.";

    public static void MapForgotPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ForgotPasswordCommandHandler handler,
                    [FromBody] ForgotPasswordCommand command) =>
            {
                await handler.Handle(command).ConfigureAwait(false);
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
