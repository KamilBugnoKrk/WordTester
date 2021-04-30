using Microsoft.EntityFrameworkCore.Migrations;

namespace MyBlazorApp.Server.Migrations
{
    public partial class SeedDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Sample course for Polish learners. Majority of Words, Definitions, Pronunciations and Example Uses come from en.wiktionary.org", "English B2+ course" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExampleUse", "Pronunciation" },
                values: new object[] { "Throwing stones at the bus is another example of your *obnoxious* behaviour.", "əbˈnɑkʃəs" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExampleUse", "Pronunciation" },
                values: new object[] { "It is a song with a *solemn* message to young people.", "ˈsɑləm" });

            migrationBuilder.InsertData(
                table: "Words",
                columns: new[] { "Id", "CourseId", "Definition", "ExampleUse", "OriginalWord", "Pronunciation", "TranslatedWord" },
                values: new object[,]
                {
                    { 18, 1, "an opinion, belief, or principle that is held as absolute truth by someone or especially an organization", "The mutability of the past is the central *tenet* of Ingsoc.", "tenet", "tɛnət", "zasada, paradygmat" },
                    { 17, 1, "to provide incentives for; to encourage", "The US government seeks to *incentivize* home ownership through a favorable tax system.", "incentivize", "ɪnˈsɛntɪvaɪz", "zachęcić" },
                    { 16, 1, "genuine; meaning what one says or does; heartfelt", "I believe he is *sincere* in his offer to help.", "sincere", "sɪnˈsɪə", "szczery" },
                    { 15, 1, "goods which are or were offered or intended for sale", "Remember that good business depends on having good *merchandise*.", "merchandise", "ˈmɜːtʃənˌdaɪz", "towar w sklepie, na targowisku" },
                    { 14, 1, "that which remains when use or need is satisfied, or when a limit is reached", "It is great that you've received a bonus - make the most of your *surplus* cash.", "surplus", "ˈsɜːpləs", "nadmiar" },
                    { 13, 1, "wearing clothes too formal", "The party will be only for my close friends, so don't be *overdressed*.", "overdressed", "ˈoʊvɚdɹɛst", "ubrany zbyt elegancko" },
                    { 12, 1, "short, coarse hair, especially on a man’s face", "I can see your *stubble* - why haven't you shaved?", "stubble", "ˈstʌb.əl", "kilkudniowy zarost" },
                    { 10, 1, "an obese person", "If you don't want to be *stout* you should avoid eating sweets.", "stout", "staʊt", "tęgi" },
                    { 9, 1, "of a near-vertical gradient", "This hill is really *steep*.", "steep", "stiːp", "stromy" },
                    { 8, 1, "out of the ordinary; odd; strange", "The sky had a *peculiar* appearance before the storm.", "peculiar", "pəˈkjuːl.jʊəɹ", "dziwny, osobliwy" },
                    { 7, 1, "after that, from then on", "He left; *thereafter* we never met again.", "thereafter", "ˌðɛəɹˈæf.tɚ", "potem, następnie" },
                    { 6, 1, "reduced to poverty or having lost a component", "English has an *impoverished* inflectional system.", "impoverished", "ɪmˈpɑvəɹɪʃt", "biedny, ubogi" },
                    { 5, 1, "to cause to become smaller", "This garment will *shrink* when wet.", "shrink", "ˈʃɹɪŋk", "zmniejszać" },
                    { 4, 1, "to make (something) larger or more important", "Could you please *magnify* this image?", "magnify", "ˈmæɡnɪfaɪ", "powiększać" },
                    { 11, 1, "having no hair", "He is a *bald* man with a moustache.", "bald", "bɔld", "łysy" },
                    { 3, 1, "unreliable, dangerous", "It is a *treacherous* mountain trail.", "treacherous", "ˈtɹɛtʃɹəs", "zdradliwy" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.UpdateData(
                table: "Courses",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "See how WordTester can be useful!", "English example course" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ExampleUse", "Pronunciation" },
                values: new object[] { "What an obnoxious smell!", "əb'nɑkʃəs" });

            migrationBuilder.UpdateData(
                table: "Words",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ExampleUse", "Pronunciation" },
                values: new object[] { "A film with a solemn social message", "'sɑləm" });
        }
    }
}
