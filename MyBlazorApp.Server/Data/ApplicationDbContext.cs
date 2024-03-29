﻿// Copyright (C) 2022  Kamil Bugno
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

using MyBlazorApp.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace MyBlazorApp.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Course>().HasData(
                new Course
                {
                    Id = 1,
                    Name = "English B2+ course",
                    Description = "Sample course for Polish learners. Majority of Words, Definitions, Pronunciations and Example Uses come from en.wiktionary.org",
                    IsVisibleForEveryone = true,
                    LanguageId = 5,
                }
            );

            builder.Entity<Language>().HasData(
               new Language
               {
                   Id = 1,
                   Name = "Croatian",
                   VoiceName = "hr-HR-GabrijelaNeural"
               },
               new Language
               {
                   Id = 2,
                   Name = "Czech",
                   VoiceName = "cs-CZ-AntoninNeural"
               },
               new Language
               {
                   Id = 3,
                   Name = "Danish",
                   VoiceName = "da-DK-ChristelNeural"
               },
               new Language
               {
                   Id = 4,
                   Name = "English - UK",
                   VoiceName = "en-GB-RyanNeural"
               },
               new Language
               {
                   Id = 5,
                   Name = "English - US",
                   VoiceName = "en-US-BrandonNeural"
               },
               new Language
               {
                   Id = 6,
                   Name = "German",
                   VoiceName = "de-DE-ConradNeural"
               },
               new Language
               {
                   Id = 7,
                   Name = "Spanish",
                   VoiceName = "es-ES-ElviraNeural"
               },
               new Language
               {
                   Id = 8,
                   Name = "Italian",
                   VoiceName = "it-IT-ElsaNeural"
               }
           );

            // Majority of Words, Definitions, Pronunciations and Example Uses come from https://en.wiktionary.org/
            // License:  Creative Commons Attribution-ShareAlike License, more info: https://creativecommons.org/licenses/by-sa/3.0/
            builder.Entity<Word>().HasData(
                new Word
                {
                    Id = 1,
                    CourseId = 1,
                    OriginalWord = "obnoxious",
                    TranslatedWord = "wstrętny, okropny",
                    Definition = "extremely unpleasant or offensive",
                    ExampleUse = "Throwing stones at the bus is another example of your *obnoxious* behaviour.",
                    Pronunciation = "əbˈnɑkʃəs"
                },
                new Word
                {
                    Id = 2,
                    CourseId = 1,
                    OriginalWord = "solemn",
                    TranslatedWord = "poważny",
                    Definition = "deeply serious and sombre",
                    ExampleUse = "It is a song with a *solemn* message to young people.",
                    Pronunciation = "ˈsɑləm"
                },
                new Word
                {
                    Id = 3,
                    CourseId = 1,
                    OriginalWord = "treacherous",
                    TranslatedWord = "zdradliwy",
                    Definition = "unreliable, dangerous",
                    ExampleUse = "It is a *treacherous* mountain trail.",
                    Pronunciation = "ˈtɹɛtʃɹəs"
                },
                new Word
                {
                    Id = 4,
                    CourseId = 1,
                    OriginalWord = "magnify",
                    TranslatedWord = "powiększać",
                    Definition = "to make (something) larger or more important",
                    ExampleUse = "Could you please *magnify* this image?",
                    Pronunciation = "ˈmæɡnɪfaɪ"
                },
                new Word
                {
                    Id = 5,
                    CourseId = 1,
                    OriginalWord = "shrink",
                    TranslatedWord = "zmniejszać",
                    Definition = "to cause to become smaller",
                    ExampleUse = "This garment will *shrink* when wet.",
                    Pronunciation = "ˈʃɹɪŋk"
                },
                new Word
                {
                    Id = 6,
                    CourseId = 1,
                    OriginalWord = "impoverished",
                    TranslatedWord = "biedny, ubogi",
                    Definition = "reduced to poverty or having lost a component",
                    ExampleUse = "English has an *impoverished* inflectional system.",
                    Pronunciation = "ɪmˈpɑvəɹɪʃt"
                },
                new Word
                {
                    Id = 7,
                    CourseId = 1,
                    OriginalWord = "thereafter",
                    TranslatedWord = "potem, następnie",
                    Definition = "after that, from then on",
                    ExampleUse = "He left; *thereafter* we never met again.",
                    Pronunciation = "ˌðɛəɹˈæf.tɚ"
                },
                new Word
                {
                    Id = 8,
                    CourseId = 1,
                    OriginalWord = "peculiar",
                    TranslatedWord = "dziwny, osobliwy",
                    Definition = "out of the ordinary; odd; strange",
                    ExampleUse = "The sky had a *peculiar* appearance before the storm.",
                    Pronunciation = "pəˈkjuːl.jʊəɹ"
                },
                new Word
                {
                    Id = 9,
                    CourseId = 1,
                    OriginalWord = "steep",
                    TranslatedWord = "stromy",
                    Definition = "of a near-vertical gradient",
                    ExampleUse = "This hill is really *steep*.",
                    Pronunciation = "stiːp"
                },
                new Word
                {
                    Id = 10,
                    CourseId = 1,
                    OriginalWord = "stout",
                    TranslatedWord = "tęgi",
                    Definition = "an obese person",
                    ExampleUse = "If you don't want to be *stout* you should avoid eating sweets.",
                    Pronunciation = "staʊt"
                },
                new Word
                {
                    Id = 11,
                    CourseId = 1,
                    OriginalWord = "bald",
                    TranslatedWord = "łysy",
                    Definition = "having no hair",
                    ExampleUse = "He is a *bald* man with a moustache.",
                    Pronunciation = "bɔld"
                },
                new Word
                {
                    Id = 12,
                    CourseId = 1,
                    OriginalWord = "stubble",
                    TranslatedWord = "kilkudniowy zarost",
                    Definition = "short, coarse hair, especially on a man’s face",
                    ExampleUse = "I can see your *stubble* - why haven't you shaved?",
                    Pronunciation = "ˈstʌb.əl"
                },
                new Word
                {
                    Id = 13,
                    CourseId = 1,
                    OriginalWord = "overdressed",
                    TranslatedWord = "ubrany zbyt elegancko",
                    Definition = "wearing clothes too formal",
                    ExampleUse = "The party will be only for my close friends, so don't be *overdressed*.",
                    Pronunciation = "ˈoʊvɚdɹɛst"
                },
                new Word
                {
                    Id = 14,
                    CourseId = 1,
                    OriginalWord = "surplus",
                    TranslatedWord = "nadmiar",
                    Definition = "that which remains when use or need is satisfied, or when a limit is reached",
                    ExampleUse = "It is great that you've received a bonus - make the most of your *surplus* cash.",
                    Pronunciation = "ˈsɜːpləs"
                },
                new Word
                {
                    Id = 15,
                    CourseId = 1,
                    OriginalWord = "merchandise",
                    TranslatedWord = "towar w sklepie, na targowisku",
                    Definition = "goods which are or were offered or intended for sale",
                    ExampleUse = "Remember that good business depends on having good *merchandise*.",
                    Pronunciation = "ˈmɜːtʃənˌdaɪz"
                },
                new Word
                {
                    Id = 16,
                    CourseId = 1,
                    OriginalWord = "sincere",
                    TranslatedWord = "szczery",
                    Definition = "genuine; meaning what one says or does; heartfelt",
                    ExampleUse = "I believe he is *sincere* in his offer to help.",
                    Pronunciation = "sɪnˈsɪə"
                },
                new Word
                {
                    Id = 17,
                    CourseId = 1,
                    OriginalWord = "incentivize",
                    TranslatedWord = "zachęcić",
                    Definition = "to provide incentives for; to encourage",
                    ExampleUse = "The US government seeks to *incentivize* home ownership through a favorable tax system.",
                    Pronunciation = "ɪnˈsɛntɪvaɪz"
                },
                new Word
                {
                    Id = 18,
                    CourseId = 1,
                    OriginalWord = "tenet",
                    TranslatedWord = "zasada, paradygmat",
                    Definition = "an opinion, belief, or principle that is held as absolute truth by someone or especially an organization",
                    ExampleUse = "The mutability of the past is the central *tenet* of Ingsoc.",
                    Pronunciation = "tɛnət"
                }
            );
            base.OnModelCreating(builder);
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<UserCourse> UserCourses { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<WordStats> WordStats { get; set; }
        public virtual DbSet<UsedRepetitionType> UsedRepetitionTypes { get; set; }
        public virtual DbSet<Language> Languages {get; set; }
        public virtual DbSet<AudioStats> AudioStats { get; set; }
        public virtual DbSet<UserCourseStats> UserCourseStats { get; set; }
    }
}
