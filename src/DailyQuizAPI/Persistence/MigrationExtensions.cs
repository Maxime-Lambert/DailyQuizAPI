using Microsoft.EntityFrameworkCore;

namespace DailyQuizAPI.Persistence;

public static class MigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QuizContext>();
        await db.Database.MigrateAsync().ConfigureAwait(false);
    }
}
