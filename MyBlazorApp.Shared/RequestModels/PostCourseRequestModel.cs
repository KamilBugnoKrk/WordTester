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

using MediatR;
using MyBlazorApp.Shared.ResponseModels;

namespace MyBlazorApp.Shared.RequestModels
{
    public class PostCourseRequestModel : IRequest<PostCourseResponseModel>
    {
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LanguageName { get; set; }
    }

    public record PostCourseRequest(string CourseId, string Name, string Description, string LanguageName);
}