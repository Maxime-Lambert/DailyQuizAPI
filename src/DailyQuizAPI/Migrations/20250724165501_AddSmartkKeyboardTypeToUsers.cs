using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSmartkKeyboardTypeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypeClavier",
                table: "AspNetUsers",
                newName: "SmartKeyboardType");

            migrationBuilder.RenameColumn(
                name: "ModeDaltonien",
                table: "AspNetUsers",
                newName: "KeyboardLayout");

            migrationBuilder.AddColumn<int>(
                name: "ColorblindMode",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorblindMode",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "SmartKeyboardType",
                table: "AspNetUsers",
                newName: "TypeClavier");

            migrationBuilder.RenameColumn(
                name: "KeyboardLayout",
                table: "AspNetUsers",
                newName: "ModeDaltonien");
        }
    }
}
