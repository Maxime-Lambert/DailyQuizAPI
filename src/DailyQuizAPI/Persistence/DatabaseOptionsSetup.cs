using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Persistence
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by dependency injection")]
    internal sealed class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
    {
        private const string _configurationSection = "DatabaseOptions";
        private readonly IConfiguration _configuration = configuration;

        public void Configure(DatabaseOptions options)
        {
            if (options is not null)
            {
                var connectionString = _configuration.GetConnectionString("Database");

                options.ConnectionString = connectionString!;

                _configuration.GetSection(_configurationSection).Bind(options);
            }
        }
    }
}
