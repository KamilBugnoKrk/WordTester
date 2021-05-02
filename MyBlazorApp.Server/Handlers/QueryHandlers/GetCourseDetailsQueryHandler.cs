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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class GetCourseDetailsQueryHandler : IRequestHandler<GetCourseDetailsRequestModel, GetCourseDetailsResponseModel>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public GetCourseDetailsQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {           
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Task<GetCourseDetailsResponseModel> Handle(GetCourseDetailsRequestModel request, CancellationToken cancellationToken)
        {
            var course = _unitOfWork.Courses.GetUserCourseWithWordsById(int.Parse(request.CourseId), request.UserId);

            if(course == null)
            {
                return Task.FromResult(new GetCourseDetailsResponseModel
                {
                    Course = null,
                    Words = null
                });
            }

            var courseDto = new CourseDto
            {
                Id = course.Id,
                Name = course.Name,
                Description = course.Description,
                IsVisibleForEveryone = course.IsVisibleForEveryone,
                NumberOfWords = course.Words.Count(),
                NumberOfKnownWords = course
                   .Words == null 
                   ? 0 
                   : course.Words.Where(w => w.WordStats?.Any(ws => ws.UserId.ToString() == request.UserId) == true).Count()
            };
            var wordsDto = _mapper.Map<IEnumerable<WordDto>>(course?.Words);

            return Task.FromResult(new GetCourseDetailsResponseModel 
            { 
                Course = courseDto, 
                Words = wordsDto
            });
        }
    }
}
