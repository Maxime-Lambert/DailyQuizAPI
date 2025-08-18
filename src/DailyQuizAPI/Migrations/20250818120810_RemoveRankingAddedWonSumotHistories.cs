using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRankingAddedWonSumotHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ranking",
                table: "SumotHistories");

            migrationBuilder.AddColumn<bool>(
                name: "Won",
                table: "SumotHistories",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Won",
                table: "SumotHistories");

            migrationBuilder.AddColumn<int>(
                name: "Ranking",
                table: "SumotHistories",
                type: "integer",
                nullable: true);
        }
    }
}
