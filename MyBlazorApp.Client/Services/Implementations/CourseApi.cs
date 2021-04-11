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

using MyBlazorApp.Client.Services.Contracts;
using MyBlazorApp.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyBlazorApp.Shared.ResponseModels;
using Flurl.Http;

namespace MyBlazorApp.Client.Services.Implementations
{
    public class CourseApi : ICourseApi
    {
        public async Task<IEnumerable<CourseDto>> GetMyCourses() =>
            (await "https://localhost:44370/api/Course/GetMyCourses"
                .GetJsonAsync<GetMyCoursesResponseModel>())
                .Courses;

        public async Task<int> PostCourse(string id, string name, string description) => 
            await $"https://localhost:44370/api/Course/PostCourse"
                .PostJsonAsync(new
                {
                    CourseId = id,
                    Name = name,
                    Description = description
                })
               .ReceiveJson<int>();

        public async Task<GetCourseDetailsResponseModel> GetCourseDetails(string courseId) =>
            await $"https://localhost:44370/api/Course/GetCourseDetails?CourseId={courseId}"
                .GetJsonAsync<GetCourseDetailsResponseModel>();
    }
}
