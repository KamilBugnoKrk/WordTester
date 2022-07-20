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

using Blazored.Modal;
using Blazored.Modal.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MyBlazorApp.Client.Modals;
using MyBlazorApp.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace MyBlazorApp.Client.Pages
{
    public partial class CourseDetails
    {
        [CascadingParameter] 
        public IModalService Modal { get; set; }
        [Parameter] 
        public string CourseId { get; set; }

        private CourseDto course;
        private IEnumerable<WordDto> words;
        private IEnumerable<WordDto> allWords;
        private string searchTerm;
        private Timer timer;
        private IEnumerable<string> languages = new List<string>();

        async void ShowEditWord(WordDto word)
        {            
            var parameters = new ModalParameters();
            parameters.Add(nameof(EditWord.Word), word);
            parameters.Add("CourseId", course.Id);
            searchTerm = string.Empty;
            var modal = Modal.Show<EditWord>(word == null ? "Add Word" : "Edit Word", parameters);
            var result = await modal.Result;
            if (!result.Cancelled && (bool)result.Data)
            {
                await GetCourseDetails();
                StateHasChanged();
            }
        }

        async Task OnSubmit()
        {
            var response = await courseApi
                .PostCourse(CourseId, course.Name, course.Description, course.SelectedLanguageName);
            
            navigationManager
                .NavigateTo("/details/" + response, true);
        }

        protected override async Task OnInitializedAsync()
        {
            timer = new Timer(600);
            timer.Elapsed += OnUserFinish;
            timer.AutoReset = false;

            if (!string.IsNullOrEmpty(CourseId))
            {
                await GetCourseDetails();
                languages = course.LanguageOptions;
            }
            else
            {
                course = new CourseDto();
                languages = await languageApi.GetAllLanguages();
            }
        }

        void HandleKeyUp(KeyboardEventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        void OnInputHandler(ChangeEventArgs e)
        {
            searchTerm = e.Value.ToString();
        }

        private void OnUserFinish(object source, ElapsedEventArgs e)
        {
            InvokeAsync(() =>
            {
                words = string.IsNullOrWhiteSpace(searchTerm) ? 
                    allWords : 
                    allWords.Where(word => word.OriginalWord.ToLower().Contains(searchTerm.ToLower()));
                StateHasChanged();
            });
        }

        private async Task GetCourseDetails()
        {
            var response = await courseApi.GetCourseDetails(CourseId);
            course = response.Course;
            allWords = response.Words;
            words = response.Words;
        }
    }
}
