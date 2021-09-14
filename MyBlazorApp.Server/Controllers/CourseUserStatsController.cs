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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using System.Threading.Tasks;
using MyBlazorApp.Shared.RequestModels;
using System;

namespace MyBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CourseUserStatsController : Controller
    {
        private readonly IMediator _mediator;

        public CourseUserStatsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMyStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _mediator.Send(new GetCourseUserStatsRequestModel { UserId = Guid.Parse(userId) });
            return Ok(response);
        }
    }
}
