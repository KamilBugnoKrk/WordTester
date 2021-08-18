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

using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MyBlazorApp.Client.Pages;
using MyBlazorApp.Client.Services.Contracts;
using MyBlazorApp.Client.States;
using MyBlazorApp.Shared;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyBlazorApp.UiTests
{
    public class DashboardTests : TestContext
    {
        [Fact(Skip = "Doesn't work at the moment because of Charts")]
        public void DisplayDefaultCourse()
        {
            MockCourseApi(true);
            MockIdentityAuthenticationStateProvider();

            var cut = RenderComponent<Dashboard>();

            Verify(cut, 0);
        }

        [Fact(Skip = "Doesn't work at the moment because of Charts")]
        public void DisplayUserCourse()
        {
            MockCourseApi(false);
            MockIdentityAuthenticationStateProvider();

            var cut = RenderComponent<Dashboard>();

            Verify(cut, 1);
        }

        private static void Verify(IRenderedComponent<Dashboard> cut, int numberOfEditIcons)
        {
            VerifyTitle(cut);
            VeryfyAddCourseTile(cut);
            VerifyEditDisplay(cut, numberOfEditIcons);
        }

        private void MockCourseApi(bool isVisibleForEveryone)
        {
            var courseApiMock = new Mock<ICourseApi>();
            List<CourseDto> courses = CreateCourses(isVisibleForEveryone);
            courseApiMock.Setup(c => c.GetMyCourses()).ReturnsAsync(courses);
            Services.AddSingleton(courseApiMock.Object);
        }

        private void MockIdentityAuthenticationStateProvider()
        {
            var stateProvider = new Mock<IdentityAuthenticationStateProvider>(new Mock<IAuthorizeApi>().Object);
            stateProvider.Setup(s => s.GetUserName()).ReturnsAsync("UserName");
            Services.AddSingleton(stateProvider.Object);
        }

        private static List<CourseDto> CreateCourses(bool isVisibleForEveryone)
        {
            return new List<CourseDto>
            {
                new CourseDto
                {
                    Id = 1,
                    Name = "Name",
                    Description = "Description",
                    IsVisibleForEveryone = isVisibleForEveryone
                }
            };
        }

        private static void VerifyEditDisplay(IRenderedComponent<Dashboard> cut, int number)
        {
            var links = cut.FindAll("a");
            var editLink = links
                .SelectMany(l => l.Attributes
                    .Where(a => a.Name == "href" && a.Value.StartsWith("/details/"))
                    .Select(a => a.Value));
            editLink.Count().Should().Be(number);
        }

        private static void VerifyTitle(IRenderedComponent<Dashboard> cut)
        {
            var allHeaders = cut.FindAll("h2");
            allHeaders.Count.Should().Be(2);
            allHeaders.Where(h => h.TextContent == "Name").Count().Should().Be(1);
            allHeaders.Where(h => h.TextContent == "Hi UserName!").Count().Should().Be(1);
        }

        private static void VeryfyAddCourseTile(IRenderedComponent<Dashboard> cut)
        {
            var addNewCourse = cut.FindAll("div.add-new");
            addNewCourse.Count.Should().Be(1);
            addNewCourse.Select(h => h.InnerHtml)
                .First().Should().Contain("Add a new course");
        }
    }
}
