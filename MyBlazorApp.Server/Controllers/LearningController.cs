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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using System.Threading.Tasks;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared;
using Microsoft.AspNetCore.Http;

namespace MyBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class LearningController : Controller
    {
        private readonly IMediator _mediator;
        
        public LearningController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetLearningRepetition(int courseId)
        {
            var requestModel = new GetLearningRepetitionRequestModel
            {
                CourseId = courseId,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };
            var response = await _mediator.Send(requestModel);
            return Ok(response);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostLearningRepetition([FromBody] PostLearningRepetitionRequest request)
        {
            var requestModel = new PostLearningRepetitionRequestModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                WordId = request.WordId,
                RepetitionType = request.RepetitionType,
                UserResponse = request.UserResponse
            };
            var response = await _mediator.Send(requestModel);
            return Ok(response);
        }
    }
}
