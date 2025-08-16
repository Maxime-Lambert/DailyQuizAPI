using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class IsDifficultAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDifficult",
                table: "Sumots",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDifficult",
                table: "Sumots");
        }
    }
}
