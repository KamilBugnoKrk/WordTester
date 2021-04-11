using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class ChangeWordStatsDefinition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NextRevision",
                table: "WordStats",
                newName: "NextRevisionTime");

            migrationBuilder.AlterColumn<long>(
                name: "RevisionFactor",
                table: "WordStats",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<long>(
                name: "NextRevisionTicks",
                table: "WordStats",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "UsedRepetitionTypes",
                table: "WordStats",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "WordStats",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WordStats_UserId",
                table: "WordStats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordStats_AspNetUsers_UserId",
                table: "WordStats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordStats_AspNetUsers_UserId",
                table: "WordStats");

            migrationBuilder.DropIndex(
                name: "IX_WordStats_UserId",
                table: "WordStats");

            migrationBuilder.DropColumn(
                name: "NextRevisionTicks",
                table: "WordStats");

            migrationBuilder.DropColumn(
                name: "UsedRepetitionTypes",
                table: "WordStats");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WordStats");

            migrationBuilder.RenameColumn(
                name: "NextRevisionTime",
                table: "WordStats",
                newName: "NextRevision");

            migrationBuilder.AlterColumn<int>(
                name: "RevisionFactor",
                table: "WordStats",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
