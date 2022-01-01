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

using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyBlazorApp.Server.Authorization;
using System.Net.Http;
using Flurl.Http;

namespace MyBlazorApp.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAuthorizator _authorizator;
        private readonly IReCaptcha _reCaptcha;

        public AuthorizeController(IReCaptcha reCaptcha, IAuthorizator authorizator)
        {
            _authorizator = authorizator;
            _reCaptcha = reCaptcha;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginParameters parameters)
        {
            var user = await _authorizator.FindByNameAsync(parameters.UserName);
            if (user == null) return BadRequest("User does not exist");
            var isSuccess = await _authorizator.CheckPasswordSignInAsync(user, parameters.Password);
            if (!isSuccess) return BadRequest("Invalid password");

            await _authorizator.SignInAsync(user, true);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterParameters parameters)
        {
            if (!await _reCaptcha.Verify(parameters.CaptchaResponse))            
                return BadRequest();            

            var user = new ApplicationUser
            {
                UserName = parameters.UserName
            };
            var (isSuccess, error) = await _authorizator.CreateAsync(user, parameters.Password);
            if (!isSuccess) return BadRequest(error);

            return await Login(new LoginParameters
            {
                UserName = parameters.UserName,
                Password = parameters.Password
            });            
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authorizator.SignOutAsync();
            return Ok();
        }

        [HttpGet]
        public UserInfo UserInfo()
        {
            return BuildUserInfo();
        }


        private UserInfo BuildUserInfo()
        {
            return new UserInfo
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserName = User.Identity.Name,
                ExposedClaims = User.Claims
                    .ToDictionary(c => c.Type, c => c.Value)
            };
        }
    }
}
