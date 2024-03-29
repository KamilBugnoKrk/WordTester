﻿// Copyright (C) 2022  Kamil Bugno
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
using System.Text;
using System;

namespace MyBlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CourseController : Controller
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetMyCourses()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _mediator.Send(new GetMyCoursesRequestModel { UserId = userId });
            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> PostCourse([FromBody] PostCourseRequest request)
        {
            var requestModel = new PostCourseRequestModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CourseId = request.CourseId,
                Description = request.Description,
                Name = request.Name,
                LanguageName = request.LanguageName
            };

            var response = await _mediator.Send(requestModel);
            return response.IsSucceed ?
              Ok(response.CourseId) :
              BadRequest(response.Error);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetCourseDetails(string courseId)
        {
            var requestModel = new GetCourseDetailsRequestModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CourseId = courseId
            };

            var response = await _mediator.Send(requestModel);

            return response.Course == null ?
                NotFound() :
                Ok(response);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> ExportCourseData(string courseId)
        {
            var requestModel = new ExportCourseDataRequestModel
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CourseId = courseId
            };

            var response = await _mediator.Send(requestModel);

            if (string.IsNullOrEmpty(response.Words))
                return NotFound();

            return File(Encoding.UTF8.GetBytes(response.Words),
                "text/json",
                $"CourseData-{DateTime.Now.ToString("MM/dd/yyyy")}.json");
        }
    }
}
