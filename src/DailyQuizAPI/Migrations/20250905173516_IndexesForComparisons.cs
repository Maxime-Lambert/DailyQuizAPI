using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class IndexesForComparisons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Sumots_Day",
                table: "Sumots",
                column: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_Sumots_Word",
                table: "Sumots",
                column: "Word");

            migrationBuilder.CreateIndex(
                name: "IX_SumotHistories_Word",
                table: "SumotHistories",
                column: "Word");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_RequesterId",
                table: "FriendRequests",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sumots_Day",
                table: "Sumots");

            migrationBuilder.DropIndex(
                name: "IX_Sumots_Word",
                table: "Sumots");

            migrationBuilder.DropIndex(
                name: "IX_SumotHistories_Word",
                table: "SumotHistories");

            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_RequesterId",
                table: "FriendRequests");
        }
    }
}
