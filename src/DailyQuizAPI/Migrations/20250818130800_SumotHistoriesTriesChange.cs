using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DailyQuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class SumotHistoriesTriesChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tries",
                table: "SumotHistories");

            migrationBuilder.CreateTable(
                name: "SumotTry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SumotHistoryId = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SumotTry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SumotTry_SumotHistories_SumotHistoryId",
                        column: x => x.SumotHistoryId,
                        principalTable: "SumotHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SumotTry_SumotHistoryId",
                table: "SumotTry",
                column: "SumotHistoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SumotTry");

            migrationBuilder.AddColumn<string>(
                name: "Tries",
                table: "SumotHistories",
                type: "jsonb",
                nullable: false,
                defaultValue: "");
        }
    }
}
