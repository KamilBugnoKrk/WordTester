using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class AddNewTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WordLearningState",
                table: "WordStats",
                newName: "RevisionFactor");

            migrationBuilder.CreateTable(
                name: "UsedRepetitionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordStatsId = table.Column<int>(type: "int", nullable: true),
                    RevisionTypeList = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsedRepetitionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsedRepetitionTypes_WordStats_WordStatsId",
                        column: x => x.WordStatsId,
                        principalTable: "WordStats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsedRepetitionTypes_WordStatsId",
                table: "UsedRepetitionTypes",
                column: "WordStatsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsedRepetitionTypes");

            migrationBuilder.RenameColumn(
                name: "RevisionFactor",
                table: "WordStats",
                newName: "WordLearningState");
        }
    }
}
