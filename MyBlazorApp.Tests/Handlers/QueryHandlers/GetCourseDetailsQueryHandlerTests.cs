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
using MyBlazorApp.Server.Handlers;
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Server;
using AutoMapper;
using MyBlazorApp.Tests.Utils;
using MyBlazorApp.Server.Handlers.QueryHandlers;

namespace MyBlazorApp.Tests
{
    public class GetCourseDetailsQueryHandlerTests
    {
        [Fact]
        public async void GetCourseDetailsQueryHandler_NoMyCourses_ReturnNull()
        {
            using var context = TestHelper.CreateInMemoryContext("GetCourseDetailsQueryHandler_NoMyCourses_ReturnNull");
            var profile = new MappingProfile();

            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MappingProfile>();
            });
            var mapper = config.CreateMapper();

            var handler = new GetCourseDetailsQueryHandler(mapper, new UnitOfWork(context));
            var request = new GetCourseDetailsRequestModel
            {
                UserId = Guid.NewGuid().ToString(),
                CourseId = "0"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.Course.Should().BeNull();
        }        
    }
}    

