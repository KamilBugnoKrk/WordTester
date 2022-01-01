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

using Xunit;
using System.Threading;
using MyBlazorApp.Shared.RequestModels;
using FluentAssertions;
using MyBlazorApp.Server.Controllers;
using MediatR;
using Moq;
using MyBlazorApp.Shared.ResponseModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using MyBlazorApp.Shared;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class CourseControllerTests
    {
        [Fact]
        public async Task GetMyCourses_NoMyCourses_ReturnNull()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m =>
                    m.Send(It.IsAny<GetMyCoursesRequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetMyCoursesResponseModel());
            var controller = new CourseController(mediatorMock.Object)
            {
                ControllerContext = MockControllerContext()
            };

            var result = await controller.GetMyCourses();
            
            ((result as OkObjectResult).Value as GetMyCoursesResponseModel).Courses.Should().BeNull();
        }

        [Fact]
        public async Task PostCourse_PostCorrectCourse_ReturnId()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m =>
                    m.Send(It.IsAny<PostCourseRequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PostCourseResponseModel { CourseId = 4, IsSucceed = true });
            var controller = new CourseController(mediatorMock.Object)
            {
                ControllerContext = MockControllerContext()
            };

            var result = await controller.PostCourse(new PostCourseRequest("0", "Name", "Description", "None"));

            (result as OkObjectResult).Value.Should().Be(4);
        }

        [Fact]
        public async Task GetCourseDetails_CorrectCourse_ReturnCourse()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m =>
                    m.Send(It.IsAny<GetCourseDetailsRequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCourseDetailsResponseModel { Course = new CourseDto() });
            var controller = new CourseController(mediatorMock.Object)
            {
                ControllerContext = MockControllerContext()
            };

            var result = await controller.GetCourseDetails("1");

            ((result as OkObjectResult).Value as GetCourseDetailsResponseModel)
                .Course.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCourseDetails_NoCourse_ReturnNoFound()
        {
            var mediatorMock = new Mock<IMediator>();
            mediatorMock
                .Setup(m =>
                    m.Send(It.IsAny<GetCourseDetailsRequestModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new GetCourseDetailsResponseModel ());
            var controller = new CourseController(mediatorMock.Object)
            {
                ControllerContext = MockControllerContext()
            };

            var result = await controller.GetCourseDetails("1");

            result.Should().BeOfType<NotFoundResult>();
        }

        private static ControllerContext MockControllerContext()
        {
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(m => m.User.IsInRole("RoleName")).Returns(true);
            httpContext.Setup(m => m.User.FindFirst(ClaimTypes.NameIdentifier)).Returns(new Claim(ClaimTypes.NameIdentifier, "userId"));

            var context = new ControllerContext(new ActionContext(httpContext.Object, new RouteData(), new ControllerActionDescriptor()));

            return context;
        }
    }
}    

