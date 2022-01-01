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
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Tests.Utils;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class GetCourseUserStatsQueryHandlerTests
    {
        [Fact]
        public async Task GetCourseUserStatsQueryHandler_NoMyCourses_ReturnEmpty()
        {
            using var context = TestHelper.CreateInMemoryContext("GetCourseUserStatsQueryHandler_NoMyCourses_ReturnEmpty");
           
            var handler = new GetCourseUserStatsQueryHandler(new UnitOfWork(context));
            var request = new GetCourseUserStatsRequestModel
            {
                UserId = Guid.NewGuid()
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.StatsForCourses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCourseUserStatsQueryHandler_OneCourse_ReturnData()
        {
            using var context = TestHelper.CreateInMemoryContext("GetCourseUserStatsQueryHandler_OneCourse_ReturnData");
            var userGuid = Guid.NewGuid();
            context.UserCourseStats.Add(
                new UserCourseStats
                {
                    UserId = userGuid,
                    Course = CreateCourse(userGuid),
                    NumberOfCorrectResponses = 10,
                    NumberOfIncorrectResponses = 11,
                    Date = DateTime.Now
                });
            context.UserCourseStats.Add(
                new UserCourseStats
                {
                    UserId = userGuid,
                    CourseId = 100,
                    NumberOfCorrectResponses = 40,
                    NumberOfIncorrectResponses = 1,
                    Date = DateTime.Now.AddDays(-1)
                });
            context.UserCourseStats.Add(
                new UserCourseStats
                {
                    UserId = userGuid,
                    CourseId = 100,
                    NumberOfCorrectResponses = 10,
                    NumberOfIncorrectResponses = 0,
                    Date = new DateTime(2020, 10, 10)
                });
            context.SaveChanges();

            var handler = new GetCourseUserStatsQueryHandler(new UnitOfWork(context));
            var request = new GetCourseUserStatsRequestModel
            {
                UserId = userGuid
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.StatsForCourses.Should().ContainKey("Name");
            var stats = response.StatsForCourses["Name"];
            stats.FirstRepetitionDate.Should().Be("Saturday, 10 October 2020");
            stats.LastThreeDays.NumberOfCorrectResponses.Should().Be(50);
            stats.LastThreeDays.NumberOfIncorrectResponses.Should().Be(12);
            stats.MonthStats.Count.Should().Be(4);
            stats.RepetitionStats.Count.Should().Be(2);
            stats.RepetitionStats.Should().ContainKeys(10, 20);
            stats.RepetitionStats[10].Should().Be(2);
            stats.RepetitionStats[20].Should().Be(1);
            stats.NewWords.Should().Be(1);
        }

        private static Course CreateCourse(Guid userGuid)
        {
            return new Course
            {
                Id = 100,
                Name = "Name",
                UserCourses = new List<UserCourse> {
                    new UserCourse
                    {
                        Id = 200,
                        UserId = userGuid,
                        CourseId = 100
                    }
                 },
                Words = CreateWords(userGuid),
            };
        }

        private static List<Word> CreateWords(Guid userGuid)
        {
            return new List<Word>
            {
                new Word
                {
                    CourseId = 100,
                    Id = 1,
                    WordStats = new List<WordStats>
                    {
                        new WordStats
                        {
                            Id = 300,
                            UserId = userGuid,
                            RevisionFactor = 10,
                            WordId = 1,
                            UpdatedTime = DateTime.UtcNow.AddDays(-1)
                        }
                    }
                },
                new Word
                {
                    CourseId = 100,
                    Id = 2,
                    WordStats = new List<WordStats>
                    {
                        new WordStats
                        {
                            Id = 400,
                            UserId = userGuid,
                            RevisionFactor = 20,
                            WordId = 2,
                            UpdatedTime = DateTime.UtcNow.AddDays(-10)
                        }
                    }
                },
                new Word
                {
                    CourseId = 100,
                    Id = 3,
                    WordStats = new List<WordStats>
                    {
                        new WordStats
                        {
                            Id = 500,
                            UserId = userGuid,
                            RevisionFactor = 10,
                            WordId = 3,
                            UpdatedTime = DateTime.UtcNow.AddDays(-2)
                        }
                    }
                },
                new Word
                {
                    CourseId = 100,
                    Id = 4
                }
            };
        }
    }
}    

