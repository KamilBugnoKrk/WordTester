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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class PostWordCommandHandler : IRequestHandler<PostWordRequestModel, PostWordResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;

        public PostWordCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PostWordResponseModel> Handle(PostWordRequestModel request, CancellationToken cancellationToken)
        {
            if (!IsUserCourse(request))
                return new PostWordResponseModel
                {
                    IsSucceed = false,
                    Error = "You can modify only your courses"
                };

            return ShouldModifyWord(request) ?
                ModifyWord(request) : 
                AddNewWord(request);
        }

        private PostWordResponseModel ModifyWord(PostWordRequestModel request)
        {
            var oldWord = _unitOfWork
                            .Words
                            .GetUserWordById(request.Word.Id, request.UserId);

            return IsModificationValid(oldWord, request.CourseId) ?
                ModifyWordInDatabase(request.Word, oldWord) :
                ReturnError(oldWord);
        }

        private PostWordResponseModel ReturnError(Word oldWord)
        {           
            return new PostWordResponseModel
            {
                IsSucceed = false,
                Error = "You can modify only words that belong to your course"
            };           
        }

        private static bool IsModificationValid(Word oldWord, string courseId) => 
            oldWord != null && oldWord.CourseId.ToString() == courseId;

        private static bool ShouldModifyWord(PostWordRequestModel request) =>
            request.Word.Id != 0;

        private PostWordResponseModel AddNewWord(PostWordRequestModel request)
        {
            var addedWord = SaveNewWordInDatabase(request);

            return new PostWordResponseModel
            {
                IsSucceed = addedWord.Entity.Id != 0
            };
        }

        private EntityEntry<Word> SaveNewWordInDatabase(PostWordRequestModel request)
        {            
            var addedWord = _unitOfWork
                            .Words.Add(new Word
                            {
                                OriginalWord = request.Word.OriginalWord,
                                TranslatedWord = request.Word.TranslatedWord,
                                Definition = request.Word.Definition,
                                ExampleUse = request.Word.ExampleUse,
                                Pronunciation = request.Word.Pronunciation,
                                CourseId = int.Parse(request.CourseId)
                            });

            _unitOfWork.Complete();
            return addedWord;
        }

        private bool IsUserCourse(PostWordRequestModel request)
        {
            var course = _unitOfWork
                .Courses
                .GetUserCourseWithWordsById(int.Parse(request.CourseId), request.UserId);

            return course != null;
        }

        private PostWordResponseModel ModifyWordInDatabase(WordDto newWord, Word word)
        {
            word.OriginalWord = newWord.OriginalWord;
            word.TranslatedWord = newWord.TranslatedWord;
            word.ExampleUse = newWord.ExampleUse;
            word.Definition = newWord.Definition;
            word.Pronunciation = newWord.Pronunciation;
            if(word.ExampleUse != newWord.ExampleUse || word.OriginalWord != newWord.OriginalWord)
            {
                word.HasAudioGenerated = false;
            }
            _unitOfWork.Complete();

            return new PostWordResponseModel
            {
                IsSucceed = true
            };
        }
    }
}
