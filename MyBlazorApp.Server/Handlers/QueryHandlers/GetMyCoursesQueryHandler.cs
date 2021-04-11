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
using Microsoft.EntityFrameworkCore;
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

            var coursesDto = _mapper.Map<IEnumerable<CourseDto>>(courses);

            return Task.FromResult(new GetMyCoursesResponseModel { Courses = coursesDto});
        }
    }
}
