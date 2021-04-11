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

using MediatR;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class PostCourseCommandHandler : IRequestHandler<PostCourseRequestModel, PostCourseResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PostCourseResponseModel> Handle(PostCourseRequestModel request, CancellationToken cancellationToken)
        {
            return ShouldModifyCourse(request) ? 
                ModifyCourse(request) : 
                AddCourse(request);
        }

        private PostCourseResponseModel AddCourse(PostCourseRequestModel request)
        {
            var courseId = SaveCourseInDatabase(request);

            return new PostCourseResponseModel
            {
                CourseId = courseId,
                IsSucceed = true
            };
        }

        private int SaveCourseInDatabase(PostCourseRequestModel request)
        {
            var addedCourse = _unitOfWork
                    .Courses.Add(new Course
                    {
                        Name = request.Name,
                        Description = request.Description,
                        IsVisibleForEveryone = false,
                        UserCourses = new List<UserCourse>
                        {
                            new UserCourse
                            {
                                UserId = new Guid(request.UserId)
                            }
                        }
                    });

            _unitOfWork.Complete();
            return addedCourse.Entity.Id;
        }

        private PostCourseResponseModel ModifyCourse(PostCourseRequestModel request)
        {
            var course = _unitOfWork
                            .Courses
                            .GetUserCourseWithWordsById(int.Parse(request.CourseId), request.UserId);

            return course == null ?
                new PostCourseResponseModel
                {
                    IsSucceed = false,
                    Error = "You can modify only your courses"
                }
                : 
                ModifyCourseInDatabase(request, course);
        }

        private PostCourseResponseModel ModifyCourseInDatabase(PostCourseRequestModel request, Course course)
        {
            course.Name = request.Name;
            course.Description = request.Description;
            _unitOfWork.Complete();

            return new PostCourseResponseModel
            {
                CourseId = course.Id,
                IsSucceed = true
            };
        }

        private static bool ShouldModifyCourse(PostCourseRequestModel request)
        {
            return !string.IsNullOrEmpty(request.CourseId);
        }
    }
}
