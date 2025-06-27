using Testcontainers.PostgreSql;

namespace DailyQuizAPI.IntegrationTests.Containers;

public static class PostgresTestContainer
{
    public static PostgreSqlContainer Create()
    {
        return new PostgreSqlBuilder()
            .WithImage("postgres:17.5-alpine3.22")
            .WithCleanUp(true)
            .WithUsername("test")
            .WithPassword("test")
            .WithDatabase("dailyquiz_test")
            .Build();
    }
}

