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

using AutoMapper;
using MediatR;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Shared;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class GetMyCoursesQueryHandler : IRequestHandler<GetMyCoursesRequestModel, GetMyCoursesResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetMyCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public Task<GetMyCoursesResponseModel> Handle(GetMyCoursesRequestModel request, CancellationToken cancellationToken)
        {
            var courses = _unitOfWork.Courses.GetMyCoursesWithWords(request.UserId);
            var userCourseStats = _unitOfWork.UserCourseStats
                    .Find(ucs => 
                            ucs.UserId.ToString() == request.UserId && 
                            ucs.Date > DateTime.UtcNow.AddDays(-7))
                    .ToList();

            var coursesDto = courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsVisibleForEveryone = c.IsVisibleForEveryone,
                NumberOfWords = c.Words.Count(),
                NumberOfKnownWords = c
                    .Words
                    .Where(w => w.WordStats.Any(ws => ws.UserId.ToString() == request.UserId))
                    .Count(),
                Stats = c.Words
                    .SelectMany(w => w.WordStats)
                    .Where(ws => ws.UserId.ToString() == request.UserId)
                    .GroupBy(ws => ws.RevisionFactor).Select(group => new
                    {
                        a = group.Key,
                        b = group.Count()
                    }).ToDictionary(o => o.a, o => o.b),
                NumberOfCorrectRepetitions = userCourseStats.Where(ucs => ucs.CourseId == c.Id).Select(ucs => new
                {
                    ucs.Date,
                    Number = (int)(ucs.NumberOfCorrectResponses + ucs.NumberOfIncorrectResponses)
                }).ToDictionary(o => o.Date, o => o.Number)
            }); 

            return Task.FromResult(new GetMyCoursesResponseModel { Courses = coursesDto});
        }
    }
}
