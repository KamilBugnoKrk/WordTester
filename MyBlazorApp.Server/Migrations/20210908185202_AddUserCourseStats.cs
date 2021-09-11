using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class AddUserCourseStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCourseStats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    NumberOfCorrectResponses = table.Column<long>(type: "bigint", nullable: false),
                    NumberOfIncorrectResponses = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCourseStats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCourseStats_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCourseStats_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseStats_CourseId",
                table: "UserCourseStats",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseStats_UserId",
                table: "UserCourseStats",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCourseStats");
        }
    }
}
