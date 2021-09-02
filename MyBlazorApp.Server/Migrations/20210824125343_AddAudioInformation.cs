using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class AddAudioInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasAudioGenerated",
                table: "Words",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LanguageName",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasAudioGenerated",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "LanguageName",
                table: "Courses");
        }
    }
}
