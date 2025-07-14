using Serilog;

namespace DailyQuizAPI.Logger;

public static class SerilogSetup
{
    public static void UseSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
    }
}
