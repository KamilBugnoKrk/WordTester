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
using MyBlazorApp.Server.Helpers;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class PostLearningRepetitionCommandHandler : IRequestHandler<PostLearningRepetitionRequestModel, PostLearningRepetitionResponseModel>
    {
        private const int _thirtySecondsInTicks = 30000000;
        private const int _numberOfRepetitionTypes = 8;
        private readonly IUnitOfWork _unitOfWork;

        public PostLearningRepetitionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public Task<PostLearningRepetitionResponseModel> Handle(PostLearningRepetitionRequestModel request, CancellationToken cancellationToken)
        {
            if (!_unitOfWork.Words.IsMyWord(request.WordId.ToString(), request.UserId))
            {
                return Task.FromResult(new PostLearningRepetitionResponseModel
                {
                    Error = "This word does not come from your courses"
                });
            }

            var (word, stats) = GetLearningData(request);

            if (stats == null)
                return AddNewWordStats(request);

            stats.UsedRepetitionTypes = UpdateRepetitionTypes(request.RepetitionType, stats.UsedRepetitionTypes);

            return IsCorrectAnswer(request, word) ?
                HandleCorrectAnswer(stats) :
                HandleIncorrectAnswer(word, stats);
        }

        private (Word word, WordStats stats) GetLearningData(PostLearningRepetitionRequestModel request)
        {
            var word = _unitOfWork.Words.GetWordWithStatsById(request.WordId);
            var stats = word.WordStats
                .Where(ws => ws.UserId.ToString() == request.UserId)
                .FirstOrDefault();
            return (word, stats);
        }

        private Task<PostLearningRepetitionResponseModel> HandleIncorrectAnswer(Word word, WordStats stats)
        {
            stats.RevisionFactor = 0;
            stats.NextRevisionTicks = _thirtySecondsInTicks;
            stats.NextRevisionTime = DateTime.Now.AddTicks(stats.NextRevisionTicks);
            _unitOfWork.Complete();

            return Task.FromResult(new PostLearningRepetitionResponseModel
            {
                IsCorrectAnswer = false,
                CorrectWord = new WordDto
                {
                    Id = word.Id,
                    Definition = word.Definition,
                    OriginalWord = word.OriginalWord,
                    TranslatedWord = word.TranslatedWord,
                    ExampleUse = Helper.ApplyStyleToText(word.ExampleUse),
                    Pronunciation = word.Pronunciation
                }
            });
        }

        private Task<PostLearningRepetitionResponseModel> HandleCorrectAnswer(WordStats stats)
        {
            stats.RevisionFactor += 1;
            stats.NextRevisionTicks *= 2;
            stats.NextRevisionTime = DateTime.Now.AddTicks(stats.NextRevisionTicks);
            _unitOfWork.Complete();

            return Task.FromResult(new PostLearningRepetitionResponseModel
            {
                IsCorrectAnswer = true
            });
        }

        private Task<PostLearningRepetitionResponseModel> AddNewWordStats(PostLearningRepetitionRequestModel request)
        {
            _unitOfWork.WordStats.Add(new WordStats
            {
                RevisionFactor = 0,
                NextRevisionTicks = _thirtySecondsInTicks,
                NextRevisionTime = DateTime.Now.AddTicks(_thirtySecondsInTicks),
                WordId = request.WordId,
                UserId = Guid.Parse(request.UserId)
            });
            _unitOfWork.Complete();

            return Task.FromResult(new PostLearningRepetitionResponseModel
            {
                IsCorrectAnswer = true
            });
        }

        private static bool IsCorrectAnswer(PostLearningRepetitionRequestModel request, Word word)
        {
            return IsOriginalWordCorrect(request, word.OriginalWord) ||
                   IsTranslatedWordCorrect(request, word.TranslatedWord) ||
                   IsDefinitionCorrect(request, word.Definition);
        }

        private static string UpdateRepetitionTypes(RepetitionType currentRepetitionType, string usedRepetitionType)
        {
            if (!HaveRepetitionType(usedRepetitionType))
            {
                var repetitionTypes = usedRepetitionType == null ? 
                    new List<RepetitionType>() :
                    JsonConvert.DeserializeObject<List<RepetitionType>>(usedRepetitionType);

                return repetitionTypes.Count == _numberOfRepetitionTypes
                    ? CreateNewRepetitionType(currentRepetitionType)
                    : AddRepetitionType(currentRepetitionType, repetitionTypes);
            }
            
            return CreateNewRepetitionType(currentRepetitionType);            
        }

        private static string AddRepetitionType(RepetitionType currentRepetitionType, List<RepetitionType> repetitionTypes)
        {
            repetitionTypes.Add(currentRepetitionType);
            return JsonConvert.SerializeObject(repetitionTypes);
        }

        private static string CreateNewRepetitionType(RepetitionType currentRepetitionType)
        {
            return JsonConvert.SerializeObject(new List<RepetitionType>
                    {
                        currentRepetitionType
                    });
        }

        private static bool HaveRepetitionType(string usedRepetitionType)
        {
            return string.IsNullOrEmpty(usedRepetitionType);
        }

        private static bool IsOriginalWordCorrect(PostLearningRepetitionRequestModel request, string originalWord)
        {
            return (request.RepetitionType == RepetitionType.FromDefinitionToOriginalClose ||
                    request.RepetitionType == RepetitionType.FromDefinitionToOriginalOpen ||
                    request.RepetitionType == RepetitionType.FromTranslatedToOriginalClose ||
                    request.RepetitionType == RepetitionType.FromTranslatedToOriginalOpen ||
                    request.RepetitionType == RepetitionType.FromExampleToOriginalClose
                    ) && request.UserResponse == originalWord;
        }

        private static bool IsTranslatedWordCorrect(PostLearningRepetitionRequestModel request, string translatedWord)
        {
            return (request.RepetitionType == RepetitionType.FromExampleToTranslatedClose ||
                    request.RepetitionType == RepetitionType.FromExampleToTranslatedOpen ||
                    request.RepetitionType == RepetitionType.FromOriginalToTranslatedClose ||
                    request.RepetitionType == RepetitionType.FromOriginalToTranslatedOpen
                    ) && request.UserResponse == translatedWord;
        }

        private static bool IsDefinitionCorrect(PostLearningRepetitionRequestModel request, string definition)
        {
            return (request.RepetitionType == RepetitionType.FromOriginalToDefinitionClose ||
                    request.RepetitionType == RepetitionType.FromExampleToDefinitionClose 
                    ) && request.UserResponse == definition;
        }
    }
}
