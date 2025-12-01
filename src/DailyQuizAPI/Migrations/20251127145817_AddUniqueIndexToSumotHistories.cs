using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToSumotHistories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""SumotHistories"" AS h
                USING (
                    SELECT ""UserId"", ""Word"", MIN(""Id"") AS ""KeepId""
                    FROM ""SumotHistories""
                    GROUP BY ""UserId"", ""Word""
                    HAVING COUNT(*) > 1
                ) AS d
                WHERE h.""UserId"" = d.""UserId""
                  AND h.""Word"" = d.""Word""
                  AND h.""Id"" <> d.""KeepId"";
                ");

            migrationBuilder.CreateIndex(
                name: "IX_SumotHistories_UserId_Word",
                table: "SumotHistories",
                columns: new[] { "UserId", "Word" },
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SumotHistories_UserId_Word",
                table: "SumotHistories"
            );
        }
    }

}
