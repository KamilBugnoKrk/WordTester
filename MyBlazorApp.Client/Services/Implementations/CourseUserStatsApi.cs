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

using MyBlazorApp.Client.Services.Contracts;
using System.Threading.Tasks;
using MyBlazorApp.Shared.ResponseModels;
using Flurl.Http;

namespace MyBlazorApp.Client.Services.Implementations
{
    public class CourseUserStatsApi : ICourseUserStatsApi
    {
        private readonly string url;

        public CourseUserStatsApi (UrlHelper urlHelper)
        {
            url = urlHelper.BaseUrl;
        }

        public async Task<GetCourseUserStatsResponseModel> GetCourseUserStats() =>
            await $"{url}api/CourseUserStats/GetMyStats"
                .GetJsonAsync<GetCourseUserStatsResponseModel>();
    }
}
