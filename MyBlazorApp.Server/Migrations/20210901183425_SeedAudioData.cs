using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class SeedAudioData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Name", "VoiceName" },
                values: new object[,]
                {
                    { 1, "Croatian (Croatia)", "hr-HR-GabrijelaNeural" },
                    { 2, "Czech (Czech)", "cs-CZ-AntoninNeural" },
                    { 3, "Danish (Denmark)", "da-DK-ChristelNeural" },
                    { 4, "English (United Kingdom)", "en-GB-RyanNeural" },
                    { 5, "English (United States)", "en-US-BrandonNeural" },
                    { 6, "German (Germany)", "de-DE-ConradNeural" },
                    { 7, "Spanish (Spain)", "es-ES-ElviraNeural" },
                    { 8, "Italian (Italy)", "it-IT-ElsaNeural" }
                });

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "LanguageId",
                value: 5);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                column: "LanguageId",
                value: null);
        }
    }
}
