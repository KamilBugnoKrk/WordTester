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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Data;
using Xunit;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Tests.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class PostCourseCommandHandlerTests
    {
        [Fact]
        public async Task PostCourseCommandHandler_AddNewCourse_CourseAdded()
        {
            using var context = TestHelper.CreateInMemoryContext("PostCourseCommandHandler_AddNewCourse_CourseAdded");
            context.Languages.Add(new Language { Name = "None", VoiceName = "None"});
            context.SaveChanges();
            var handler = new PostCourseCommandHandler(new UnitOfWork(context));
            var request = new PostCourseRequestModel
            {
                Name = "name",
                Description = "desc",
                UserId = Guid.NewGuid().ToString(),
                LanguageName = "None"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.CourseId.Should().BeGreaterThan(0);
            (await context.Courses.CountAsync()).Should().Be(1);
            var addedCourse = await context.Courses.FirstAsync();
            addedCourse.Id.Should().Be(response.CourseId);

        }

        [Fact]
        public async Task PostCourseCommandHandler_UpdateCourse_CourseUpdated()
        {
            var userGuid = Guid.NewGuid();
            using var context = TestHelper.CreateInMemoryContext("PostCourseCommandHandler_UpdateCourse_CourseUpdated");
            SeedDatabase(userGuid, context);
            var oldCourse = await context.Courses.FirstAsync();
            var handler = new PostCourseCommandHandler(new UnitOfWork(context));
            var request = new PostCourseRequestModel
            {
                CourseId = oldCourse.Id.ToString(),
                Name = "new name",
                Description = "new description",
                LanguageName = "old language",
                UserId = userGuid.ToString()
            };

            var response = await handler.Handle(request, CancellationToken.None);

            (await context.Courses.CountAsync()).Should().Be(1);
            response.CourseId.Should().Be(oldCourse.Id);
            var updatedCourse = await context.Courses.FirstAsync();
            updatedCourse.Name.Should().Be("new name");
            updatedCourse.Description.Should().Be("new description");
            var word = updatedCourse.Words.First();
            word.HasAudioGenerated.Should().BeTrue();
        }

        [Fact]
        public async Task PostCourseCommandHandler_UpdateCourseAndChangeTheLanguage_CourseAndWordsUpdated()
        {
            var userGuid = Guid.NewGuid();
            using var context = TestHelper.CreateInMemoryContext(
                "PostCourseCommandHandler_UpdateCourseAndChangeTheLanguage_CourseAndWordsUpdated");
            SeedDatabase(userGuid, context);
            var oldCourse = await context.Courses.FirstAsync();
            var handler = new PostCourseCommandHandler(new UnitOfWork(context));
            var request = new PostCourseRequestModel
            {
                CourseId = oldCourse.Id.ToString(),
                Name = "new name",
                Description = "new description",
                UserId = userGuid.ToString(),
                LanguageName = "new language"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            (await context.Courses.CountAsync()).Should().Be(1);
            response.CourseId.Should().Be(oldCourse.Id);
            var updatedCourse = await context.Courses.FirstAsync();
            updatedCourse.Name.Should().Be("new name");
            updatedCourse.Description.Should().Be("new description");
            var word = updatedCourse.Words.First();
            word.HasAudioGenerated.Should().BeFalse();
        }

        private static void SeedDatabase(Guid userGuid, ApplicationDbContext context)
        {
            context.Languages.Add(new Language
            {
                Id = 11,
                Name = "new language",
                VoiceName = "voice"
            });
            context.Courses.Add(new Course
            {
                Id = 10,
                Name = "old name",
                Description = "old description",
                IsVisibleForEveryone = false,
                UserCourses = new List<UserCourse>
                {
                    new UserCourse
                    {
                        UserId = userGuid
                    }
                },
                Language = new Language
                {
                    Id = 10,
                    Name = "old language",
                    VoiceName = "voice"
                },
                Words = new List<Word> { 
                    new Word 
                    { 
                        CourseId = 10, 
                        OriginalWord = "OriginalWord",
                        TranslatedWord = "TranslatedWord",
                        ExampleUse = "ExampleUse",
                        Definition = "Definition",
                        HasAudioGenerated = true
                    } 
                }
            });
            context.SaveChanges();
        }
    }
}    

