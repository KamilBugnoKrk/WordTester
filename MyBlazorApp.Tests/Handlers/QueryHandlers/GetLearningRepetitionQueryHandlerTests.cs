// Copyright (C) 2022  Kamil Bugno
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

using MyBlazorApp.Server.Data;
using Xunit;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using MyBlazorApp.Server;
using AutoMapper;
using MyBlazorApp.Tests.Utils;
using MyBlazorApp.Server.LearningAlgorithm;
using Moq;
using MyBlazorApp.Shared;
using MyBlazorApp.Server.Models;
using System.Collections.Generic;
using MyBlazorApp.Server.Data.Audio;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class GetLearningRepetitionQueryHandlerTests
    {
        [Fact]
        public async Task GetLearningRepetitionQueryHandler_NoMyCourses_ReturnError()
        {
            using var context = TestHelper.CreateInMemoryContext("GetLearningRepetitionQueryHandler_NoMyCourses_ReturnError");
            var (handler, request) = CreateDependencies(context, Guid.NewGuid(), 100);

            var response = await handler.Handle(request, CancellationToken.None);

            response.ResponseType.Should().Be(ResponseType.ErrorResponse);
        }

        [Fact]
        public async Task GetLearningRepetitionQueryHandler_NoWordLearned_ReturnWordInformation()
        {
            using var context = TestHelper.CreateInMemoryContext("GetLearningRepetitionQueryHandler_NoWordLearned_ReturnWordInformation");
            var userId = Guid.NewGuid();
            AddDataToDatabase(userId, context);
            var (handler, request) = CreateDependencies(context, userId);

            var response = await handler.Handle(request, CancellationToken.None);

            response.ResponseType.Should().Be(ResponseType.InformationalResponse);
            response.InformationalWord.Id.Should().Be(20);
            response.InformationalWord.OriginalWord.Should().Be("OriginalWord");
            response.InformationalWord.TranslatedWord.Should().Be("TranslatedWord");
            response.InformationalWord.Definition.Should().Be("Definition");
            response.InformationalWord.ExampleUse.Should().Be("ExampleUse");
            response.InformationalWord.Pronunciation.Should().Be("Pronunciation");
        }

        [Fact]
        public async Task GetLearningRepetitionQueryHandler_WordLearned_ReturnWordRepetition()
        {
            using var context = TestHelper.CreateInMemoryContext("GetLearningRepetitionQueryHandler_WordLearned_ReturnWordRepetition");
            var userId = Guid.NewGuid();
            AddDataToDatabase(userId, context, true);
            var (handler, request) = CreateDependencies(context, userId);

            var response = await handler.Handle(request, CancellationToken.None);

            response.ResponseType.Should().Be(ResponseType.PracticeResponse);
            response.RepetitionType.Should().Be(RepetitionType.FromExampleToTranslatedOpen);
            response.Question.Should().Be("Question");
            response.Responses.Should().BeNull();
        }

        [Fact]
        public async Task GetLearningRepetitionQueryHandler_AllWordLearned_ReturnNothingToLearn()
        {
            using var context = TestHelper.CreateInMemoryContext("GetLearningRepetitionQueryHandler_AllWordLearned_ReturnNothingToLearn");
            var userId = Guid.NewGuid();
            AddDataToDatabase(userId, context, true, false);            
            var (handler, request) = CreateDependencies(context, userId);

            var response = await handler.Handle(request, CancellationToken.None);

            response.ResponseType.Should().Be(ResponseType.NothingToLearnResponse);
        }

        private static (GetLearningRepetitionQueryHandler handler, GetLearningRepetitionRequestModel request) CreateDependencies(ApplicationDbContext context, Guid userId, int courseId = 10)
        {
            var profile = new MappingProfile();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();

            var repetitionManagerMock = new Mock<IRepetitionManager>();
            repetitionManagerMock
                .Setup(rm => rm.CreateRepetitionData(It.Is<WordStats>(ws => ws.Id == 30)))
                .Returns(("Question",null, null, null, RepetitionType.FromExampleToTranslatedOpen));

            var handler = new GetLearningRepetitionQueryHandler(new UnitOfWork(context),
                mapper, repetitionManagerMock.Object, new Mock<IAudioService>().Object);

            var request = new GetLearningRepetitionRequestModel
            {
                CourseId = courseId,
                UserId = userId.ToString()
            };
            return (handler, request);
        }

        private static void AddDataToDatabase(Guid userGuid, ApplicationDbContext context, bool isWordLearned = false, bool isToRepeat = true)
        {
            context.Courses.Add(new Course
            {
                Id = 10,
                Name = "name",
                Description = "description",
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
                        Id = 20,
                        CourseId = 10,
                        OriginalWord = "OriginalWord",
                        TranslatedWord = "TranslatedWord",
                        Definition = "Definition",
                        ExampleUse = "ExampleUse",
                        Pronunciation = "Pronunciation",
                        WordStats = isWordLearned ? 
                            new List<WordStats>
                            {
                                new WordStats
                                {
                                    Id = 30,
                                    WordId = 20,
                                    UserId = userGuid,
                                    RevisionFactor = 8,
                                    NextRevisionTime = isToRepeat ? DateTime.MinValue : DateTime.MaxValue,
                                    NextRevisionTicks = 3000,
                                    UsedRepetitionTypes = "[1,2,3]"
                                }
                            } 
                            :
                            null
                    }
                }
            });
            context.SaveChanges();
        }
    }
}    

