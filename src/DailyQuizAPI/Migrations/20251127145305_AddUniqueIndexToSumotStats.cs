using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToSumotStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM ""SumotStats"" AS s
                USING (
                    SELECT ""Date"", ""IsMobile"", MIN(""Id"") AS ""KeepId""
                    FROM ""SumotStats""
                    GROUP BY ""Date"", ""IsMobile""
                    HAVING COUNT(*) > 1
                ) AS d
                WHERE s.""Date"" = d.""Date""
                  AND s.""IsMobile"" = d.""IsMobile""
                  AND s.""Id"" <> d.""KeepId"";
            ");

            migrationBuilder.CreateIndex(
                name: "IX_SumotStats_Date_IsMobile",
                table: "SumotStats",
                columns: new[] { "Date", "IsMobile" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SumotStats_Date_IsMobile",
                table: "SumotStats"
            );
        }
    }
}
