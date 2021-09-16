// Copyright (C) 2021  Kamil Bugno
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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Data;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;
using MyBlazorApp.Server.Controllers;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Tests.Utils;
using MyBlazorApp.Shared;
using MyBlazorApp.Server.Data.Audio;
using Moq;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class PostLearningRepetitionQueryHandlerTests
    {
        [Fact]
        public async Task PostLearningRepetitionQueryHandler_FirstCorrectRepetition_WordStatsCreated()
        {
            using var context = TestHelper.CreateInMemoryContext("PostLearningRepetitionQueryHandler_FirstCorrectRepetition_WordStatsCreated");
            (var request, var handler) = SetupDependencies(context);

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsCorrectAnswer.Should().BeTrue();
            (await context.WordStats.CountAsync()).Should().Be(1);
            var wordStats = await context.WordStats.FirstOrDefaultAsync();
            wordStats.RevisionFactor.Should().Be(0);
            wordStats.UsedRepetitionTypes.Should().BeNull();
        }

        [Fact]
        public async Task PostLearningRepetitionQueryHandler_SecondCorrectRepetition_WordStatsUpdated()
        {
            using var context = TestHelper.CreateInMemoryContext("PostLearningRepetitionQueryHandler_SecondCorrectRepetition_WordStatsCreated");
            (var request, var handler) = SetupDependencies(context, true);

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsCorrectAnswer.Should().BeTrue();
            (await context.WordStats.CountAsync()).Should().Be(1);
            var wordStats = await context.WordStats.FirstOrDefaultAsync();
            wordStats.RevisionFactor.Should().Be(1);
            wordStats.UsedRepetitionTypes.Should().Be("[7]");
        }

        [Fact]
        public async Task PostLearningRepetitionQueryHandler_AllRepetitionTypesUsed_WordStatsUpdated()
        {
            using var context = TestHelper.CreateInMemoryContext("PostLearningRepetitionQueryHandler_AllRepetitionTypesUsed_WordStatsUpdated");
            (var request, var handler) = SetupDependencies(context, true, "[1,2,3,4,5,6,7,8]");

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsCorrectAnswer.Should().BeTrue();
            (await context.WordStats.CountAsync()).Should().Be(1);
            var wordStats = await context.WordStats.FirstOrDefaultAsync();
            wordStats.RevisionFactor.Should().Be(2);
            wordStats.UsedRepetitionTypes.Should().Be("[7]");
        }

        [Fact]
        public async Task PostLearningRepetitionQueryHandler_IncorrectResponse_WordStatsUpdated()
        {
            using var context = TestHelper.CreateInMemoryContext("PostLearningRepetitionQueryHandler_IncorrectResponse_WordStatsUpdated");
            (var request, var handler) = SetupDependencies(context, true, "[1,2,3,4,5,8]", false);

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsCorrectAnswer.Should().BeFalse();
            (await context.WordStats.CountAsync()).Should().Be(1);
            var wordStats = await context.WordStats.FirstOrDefaultAsync();
            wordStats.RevisionFactor.Should().Be(0);
            wordStats.UsedRepetitionTypes.Should().Be("[1,2,3,4,5,8,7]");
        }

        private static (PostLearningRepetitionRequestModel request, PostLearningRepetitionCommandHandler handler)
            SetupDependencies(ApplicationDbContext context, bool hasStats = false, string usedRepetitionTypes = null, bool isCorrect = true)
        {
            var userId = Guid.NewGuid();
            CreateDataInDatabase(userId, context, hasStats, usedRepetitionTypes);
            var handler = new PostLearningRepetitionCommandHandler(new UnitOfWork(context), new Mock<IAudioService>().Object);
            var request = new PostLearningRepetitionRequestModel
            {
                RepetitionType = RepetitionType.FromDefinitionToOriginalClose,
                UserResponse = isCorrect ? "OriginalWord" : "NotOriginalWord",
                WordId = 1,
                UserId = userId.ToString()
            };
            return (request, handler);
        }

        private static void CreateDataInDatabase(Guid userGuid, ApplicationDbContext context, bool hasStats = false, string usedRepetitionTypes = null)
        {
            context.Courses.Add(new Course
            {
                Id = 1,
                Name = "MyCourse",
                Description = "MyDescription",
                IsVisibleForEveryone = false,
                UserCourses = new List<UserCourse>
                {
                    new UserCourse
                    {
                        UserId = userGuid
                    }
                },
                Words = new List<Word>
                {
                    new Word
                    {
                        Id = 1,
                        OriginalWord = "OriginalWord",
                        TranslatedWord = "TranslatedWord",
                        Definition = "Definition",
                        ExampleUse = "ExampleUse",
                        Pronunciation = "Pronunciation",
                        WordStats = hasStats ? new List<WordStats>
                        {
                            new WordStats
                            {
                                Id = 1,
                                WordId = 1,
                                RevisionFactor = usedRepetitionTypes == null ? 0 : 1,
                                NextRevisionTime = DateTime.MinValue,
                                NextRevisionTicks = 30000000,
                                UsedRepetitionTypes = usedRepetitionTypes,
                                UserId = userGuid
                            }
                        }:
                        null
                    }
                }
            });
            context.SaveChanges();
        }
    }
}    

