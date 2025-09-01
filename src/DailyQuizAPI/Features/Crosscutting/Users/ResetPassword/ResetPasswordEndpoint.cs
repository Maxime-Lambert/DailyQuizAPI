namespace DailyQuizAPI.Features.Crosscutting.Users.ResetPassword;

using DailyQuizAPI.OpenApi;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

public static class ResetPasswordEndpoint
{
    private const string ROUTE = "/users/resetpassword";
    private const string NAME = "ResetPassword";
    private const string TAG = "Users";
    private const string SUMMARY = "Renseignes un nouveau mot de passe.";
    private const string DESCRIPTION = "Mets à jour le mot de passe d’un utilisateur à l’aide du lien reçu par mail.";
    private const string OPERATION_ID = "Users_ResetPassword";
    private const string SUCCESS_DESCRIPTION = "Mot de passe mis à jour avec succès.";

    public static void MapResetPasswordEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(ROUTE,
            async ([FromServices] ResetPasswordCommandHandler handler,
                   [FromBody] ResetPasswordCommand command) =>
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

