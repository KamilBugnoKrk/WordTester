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
using MyBlazorApp.Server.Controllers;
using Moq;
using MyBlazorApp.Shared;
using MyBlazorApp.Server.Models;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using MyBlazorApp.Server.Authorization;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class AuthorizeControllerTest
    {
        [Fact]
        public async Task Login_UserNotExists_ReturnsBadRequest()
        {
            var authorizatorMock = new Mock<IAuthorizator>().Object;
            var reCaptcha = new Mock<IReCaptcha>().Object;
            var controller = new AuthorizeController(reCaptcha, authorizatorMock);

            var result = await controller.Login(new LoginParameters());

            (result as BadRequestObjectResult).Value.Should().Be("User does not exist");
        }

        [Fact]
        public async Task Login_BadPassword_ReturnsBadRequest()
        {
            var controller = CreateDependenciesForLogin(false);

            var result = await controller.Login(new LoginParameters());

            (result as BadRequestObjectResult).Value.Should().Be("Invalid password");
        }

        [Fact]
        public async Task Login_CorrectPassword_ReturnsOk()
        {
            var controller = CreateDependenciesForLogin(true);

            var result = await controller.Login(new LoginParameters());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Register_ErrorDuringCreation_ReturnsBadRequest()
        {
            var controller = CreateDependenciesForRegister(false, "MyErrorDescription");

            var result = await controller.Register(new RegisterParameters());

            (result as BadRequestObjectResult).Value.Should().Be("MyErrorDescription");
        }

        [Fact]
        public async Task Register_CorrectCreation_ReturnsOk()
        {
            var controller = CreateDependenciesForRegister(true, string.Empty);

            var result = await controller.Register(new RegisterParameters());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task Logout_CorrectLogout_ReturnsOk()
        {
            var authorizatorMock = new Mock<IAuthorizator>().Object;
            var reCaptcha = new Mock<IReCaptcha>().Object;
            var controller = new AuthorizeController(reCaptcha, authorizatorMock);

            var result = await controller.Logout();

            result.Should().BeOfType<OkResult>();
        }

        private static AuthorizeController CreateDependenciesForRegister(bool isSuccess, string error)
        {
            var authorizatorMock = new Mock<IAuthorizator>();
            authorizatorMock
                .Setup(a => a.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((isSuccess, error));
            authorizatorMock
               .Setup(a => a.FindByNameAsync(It.IsAny<string>()))
               .ReturnsAsync(new ApplicationUser());
            authorizatorMock
                .Setup(a => a.CheckPasswordSignInAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(true);

            var reCaptcha = new Mock<IReCaptcha>();
            reCaptcha.Setup(rc => rc.Verify(It.IsAny<string>())).ReturnsAsync(true);

            var controller = new AuthorizeController(reCaptcha.Object, authorizatorMock.Object);
            return controller;
        }

        private static AuthorizeController CreateDependenciesForLogin(bool isCorrectPassword)
        {
            var authorizatorMock = new Mock<IAuthorizator>();
            authorizatorMock
                .Setup(a => a.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(new ApplicationUser());
            authorizatorMock
                .Setup(a => a.CheckPasswordSignInAsync(
                    It.IsAny<ApplicationUser>(),
                    It.IsAny<string>()))
                .ReturnsAsync(isCorrectPassword);
            var controller = new AuthorizeController(new Mock<IReCaptcha>().Object, authorizatorMock.Object);
            return controller;
        }
    }
}    

