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

namespace MyBlazorApp.Tests
{
    public class PostCourseCommandHandlerTests
    {
        [Fact]
        public async void PostCourseCommandHandler_AddNewCourse_CourseAdded()
        {
            using var context = TestHelper.CreateInMemoryContext("PostCourseCommandHandler_AddNewCourse_CourseAdded");
            var handler = new PostCourseCommandHandler(new UnitOfWork(context));
            var request = new PostCourseRequestModel
            {
                Name = "name",
                Description = "desc",
                UserId = Guid.NewGuid().ToString()
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.CourseId.Should().BeGreaterThan(0);
            (await context.Courses.CountAsync()).Should().Be(1);
            var addedCourse = await context.Courses.FirstAsync();
            addedCourse.Id.Should().Be(response.CourseId);

        }

        [Fact]
        public async void PostCourseCommandHandler_UpdateCourse_CourseUpdated()
        {
            var userGuid = Guid.NewGuid();
            using var context = TestHelper.CreateInMemoryContext("PostCourseCommandHandler_UpdateCourse_CourseUpdated");
            AddCourseToDatabase(userGuid, context);
            var oldCourse = await context.Courses.FirstAsync();
            var handler = new PostCourseCommandHandler(new UnitOfWork(context));
            var request = new PostCourseRequestModel
            {
                CourseId = oldCourse.Id.ToString(),
                Name = "new name",
                Description = "new description",
                UserId = userGuid.ToString()
            };

            var response = await handler.Handle(request, CancellationToken.None);

            var updatedCourse = await context.Courses.FirstAsync();
            (await context.Courses.CountAsync()).Should().Be(1);
            response.CourseId.Should().Be(oldCourse.Id);
            updatedCourse.Name.Should().Be("new name");
            updatedCourse.Description.Should().Be("new description");
        }

        private static void AddCourseToDatabase(Guid userGuid, ApplicationDbContext context)
        {
            context.Courses.Add(new Course
            {
                Name = "old name",
                Description = "old description",
                IsVisibleForEveryone = false,
                UserCourses = new List<UserCourse>
                {
                    new UserCourse
                    {
                        UserId = userGuid
                    }
                }
            });
            context.SaveChanges();
        }
    }
}    

