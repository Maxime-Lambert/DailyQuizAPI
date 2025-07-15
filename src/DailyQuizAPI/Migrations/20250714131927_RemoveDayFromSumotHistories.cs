using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDayFromSumotHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "SumotHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "Day",
                table: "SumotHistories",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }
    }
}
