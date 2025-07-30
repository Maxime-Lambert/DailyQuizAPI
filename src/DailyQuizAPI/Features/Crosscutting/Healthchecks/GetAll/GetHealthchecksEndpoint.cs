using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace DailyQuizAPI.Features.Crosscutting.Healthchecks.GetAll;

public static class GetHealthchecksEndpoint
{
    private const string HEALTHCHECKS_ENDPOINT_ROUTE = "/healthchecks";

    public static void MapGetHealthchecks(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks(HEALTHCHECKS_ENDPOINT_ROUTE, new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        })
        .WithName("GetHealth")
        .WithTags("Healthchecks");
    }
}