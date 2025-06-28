using Microsoft.AspNetCore.Mvc;

namespace DailyQuizAPI.AppSettings.CreateAppSetting;

public static class CreateAppSettingEndpoint
{
    private const string APP_SETTINGS_ENDPOINT_ROUTE = "/appsettings";

    public static void MapPostAppSettingEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost(APP_SETTINGS_ENDPOINT_ROUTE,
            async ([FromBody] CreateAppSettingCommand command, [FromServices] CreateAppSettingCommandHandler handler, CancellationToken ct) =>
            {
                await handler.Handle(command, ct).ConfigureAwait(false);
                return Results.Created();
            })
        .WithName("CreateAppSetting")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .WithTags("AppSetting")
        .WithOpenApi();
    }

}