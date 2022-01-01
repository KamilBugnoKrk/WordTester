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
    public class ExportCourseDataQueryHandler : IRequestHandler<ExportCourseDataRequestModel, ExportCourseDataResponseModel>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ExportCourseDataQueryHandler(IMapper mapper, IUnitOfWork unitOfWork)
        {           
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public Task<ExportCourseDataResponseModel> Handle(ExportCourseDataRequestModel request, CancellationToken cancellationToken)
        {
            var course = _unitOfWork.Courses.GetUserCourseWithWordsById(int.Parse(request.CourseId), request.UserId);

            if(course == null)
            {
                return Task.FromResult(new ExportCourseDataResponseModel
                {
                    Words = null
                });
            }

            var words = course?.Words.Select(w => new { 
                originalWord = w.OriginalWord,
                translatedWord = w.TranslatedWord,
                pronunciation = w.Pronunciation,
                exampleUse = w.ExampleUse,
                definition = w.Definition
            });

            return Task.FromResult(new ExportCourseDataResponseModel
            { 
                Words = Newtonsoft.Json.JsonConvert.SerializeObject(words)
            });
        }
    }
}
