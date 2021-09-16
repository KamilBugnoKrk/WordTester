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
using System.Threading.Tasks;
using MyBlazorApp.Shared.ResponseModels;
using Flurl.Http;
using MyBlazorApp.Shared.RequestModels;

namespace MyBlazorApp.Client.Services.Implementations
{
    public class WordApi : IWordApi
    {
        private readonly string url;
        public WordApi(UrlHelper urlHelper)
        {
            url = urlHelper.BaseUrl;
        }
        public async Task<PostWordResponseModel> PostWord(WordDto word, int courseId)        
             => await $"{url}api/Word/PostWord"
                .PostJsonAsync(new PostWordRequestModel{ 
                    Word = word,
                    CourseId = courseId.ToString()
                })
                .ReceiveJson<PostWordResponseModel>();

        public async Task<DeleteWordResponseModel> DeleteWord(int wordId)
            => await $"{url}api/Word/DeleteWord?wordId={wordId}"
               .DeleteAsync()
               .ReceiveJson<DeleteWordResponseModel>();
    }
}
