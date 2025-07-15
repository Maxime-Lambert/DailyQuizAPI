using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class CollectionsCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tries",
                table: "SumotHistory",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tries",
                table: "SumotHistory");
        }
    }
}
