namespace DailyQuizAPI.Persistence
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Used by dependency injection")]
    internal sealed class DatabaseOptions
    {
        public string? ConnectionString { get; set; }

        public int MaxRetryCount { get; set; }

        public int CommandTimeout { get; set; }

        public bool EnableDetailedErrors { get; set; }

        public bool EnableSensitiveDataLogging { get; set; }
    }
}
