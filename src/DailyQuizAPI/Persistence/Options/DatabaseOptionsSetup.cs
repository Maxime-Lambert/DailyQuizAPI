using Microsoft.Extensions.Options;

namespace DailyQuizAPI.Persistence.Options
{
    public sealed class DatabaseOptionsSetup(IConfiguration configuration) : IConfigureOptions<DatabaseOptions>
    {
        private const string _configurationSection = "DatabaseOptions";
        private readonly IConfiguration _configuration = configuration;

        public void Configure(DatabaseOptions options)
        {
            var connectionString = _configuration.GetConnectionString("Database");

            options.ConnectionString = connectionString!;

            _configuration.GetSection(_configurationSection).Bind(options);
        }
    }
}
