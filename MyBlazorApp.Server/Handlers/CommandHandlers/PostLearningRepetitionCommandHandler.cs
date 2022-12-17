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
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Data.Audio;
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
        private const int _fiveSecondsInTicks = 50000000;
        private const long _thirtyMinutesInTicks = 18000000000;
        private const long _threeDaysInTicks = 2592000000000;
        private const long _ninetyDaysInTicks = 77760000000000;
        private const long _thirtyDaysInTicks = 25920000000000;
        private const int _numberOfRepetitionTypes = 8;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAudioService _audioService;

        public PostLearningRepetitionCommandHandler(IUnitOfWork unitOfWork, IAudioService audioService)
        {
            _unitOfWork = unitOfWork;
            _audioService = audioService;
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
                HandleCorrectAnswer(stats, Guid.Parse(request.UserId)) :
                HandleIncorrectAnswer(word, stats, Guid.Parse(request.UserId));
        }

        private (Word word, WordStats stats) GetLearningData(PostLearningRepetitionRequestModel request)
        {
            var word = _unitOfWork.Words.GetWordWithStatsById(request.WordId);
            var stats = word.WordStats
                .FirstOrDefault(ws => ws.UserId.ToString() == request.UserId);
            return (word, stats);
        }

        private Task<PostLearningRepetitionResponseModel> HandleIncorrectAnswer(Word word, WordStats stats, Guid userId)
        {
            stats.RevisionFactor = 0;
            stats.NextRevisionTicks = _fiveSecondsInTicks;
            stats.NextRevisionTime = DateTime.Now.AddTicks(stats.NextRevisionTicks);
            stats.UpdatedTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);
            var userCourseStats = RetrieveUserCourseStats(stats, userId);
            if (userCourseStats == null)
            {
                _unitOfWork.UserCourseStats
                    .Add(new UserCourseStats
                    {
                        Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0),
                        CourseId = word.CourseId,
                        UserId = userId,
                        NumberOfIncorrectResponses = 1
                    });
            }
            else
            {
                userCourseStats.NumberOfIncorrectResponses += 1;
                _unitOfWork.UserCourseStats
                    .Update(userCourseStats);
            }
            _unitOfWork.Complete();

            string audio = string.Empty;

            if (word.HasAudioGenerated)
            {
                audio = _audioService.RetrieveAudio(word.Id, WordType.OriginalWord);
            }

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
                },
                Audio = audio
            });
        }

        private UserCourseStats RetrieveUserCourseStats(WordStats stats, Guid userId)
        { 
            var today = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0);
            return _unitOfWork.UserCourseStats
                .Find(ucs =>
                    ucs.CourseId == stats.Word.CourseId &&
                    ucs.UserId == userId &&
                    ucs.Date == today)
                .FirstOrDefault();
        }

        private Task<PostLearningRepetitionResponseModel> HandleCorrectAnswer(WordStats stats, Guid userId)
        {
            stats.RevisionFactor += 1;
            stats.NextRevisionTicks = GetNextRevisionTicks(stats.NextRevisionTicks);
            stats.NextRevisionTime = DateTime.Now.AddTicks(stats.NextRevisionTicks);
            stats.UpdatedTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0);
            var userCourseStats = RetrieveUserCourseStats(stats, userId);
            if (userCourseStats == null)
            {
                _unitOfWork.UserCourseStats
                    .Add(new UserCourseStats
                    {
                        Date = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0),
                        CourseId = stats.Word.CourseId,
                        UserId = userId,
                        NumberOfCorrectResponses = 1
                    });
            }
            else
            {
                userCourseStats.NumberOfCorrectResponses += 1;
                _unitOfWork.UserCourseStats
                    .Update(userCourseStats);
            }
            _unitOfWork.Complete();

            return Task.FromResult(new PostLearningRepetitionResponseModel
            {
                IsCorrectAnswer = true
            });
        }

        private static long GetNextRevisionTicks(long currentRevisionTicks)
        {
            if (currentRevisionTicks > _ninetyDaysInTicks)
            {
                return currentRevisionTicks * (long)1.3;
            }
            else if (currentRevisionTicks > _thirtyDaysInTicks)
            {
                return currentRevisionTicks * (long)1.6;
            }
            else if (currentRevisionTicks > _threeDaysInTicks)
            {
                return currentRevisionTicks * (long)1.8;
            }
            else if (currentRevisionTicks > _thirtyMinutesInTicks)
            {
                return currentRevisionTicks * (long)4.5;
            }
            else
            {
                return currentRevisionTicks * (long)2.5;
            }
        }

        private Task<PostLearningRepetitionResponseModel> AddNewWordStats(PostLearningRepetitionRequestModel request)
        {
            _unitOfWork.WordStats.Add(new WordStats
            {
                RevisionFactor = 0,
                NextRevisionTicks = _fiveSecondsInTicks,
                NextRevisionTime = DateTime.Now.AddTicks(_fiveSecondsInTicks),
                WordId = request.WordId,
                UserId = Guid.Parse(request.UserId),
                UpdatedTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, 0, 0)
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
                   IsOriginalWordCorrect(request, Helper.GetOriginalFromExample(word.ExampleUse)) ||
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
                    request.RepetitionType == RepetitionType.FromExampleToOriginalClose ||
                    request.RepetitionType == RepetitionType.FromTranslatedToOriginalDifferentClose ||
                    request.RepetitionType == RepetitionType.FromDefinitionToOriginalDifferentClose
                    ) && (request.UserResponse.ToLower().Trim() == originalWord.ToLower().Trim() || (Helper.GetWithoutAbbreviations(originalWord).ToLower().Trim() == request.UserResponse.ToLower().Trim() && request.UserResponse.Trim().Length > 0));
        }

        private static bool IsTranslatedWordCorrect(PostLearningRepetitionRequestModel request, string translatedWord)
        {
            return (request.RepetitionType == RepetitionType.FromExampleToTranslatedClose ||
                    request.RepetitionType == RepetitionType.FromOriginalToTranslatedClose
                    ) && Helper.IsCorrectTranslation(translatedWord, request.UserResponse);
        }

        private static bool IsDefinitionCorrect(PostLearningRepetitionRequestModel request, string definition)
        {
            return (request.RepetitionType == RepetitionType.FromOriginalToDefinitionClose ||
                    request.RepetitionType == RepetitionType.FromExampleToDefinitionClose 
                    ) && request.UserResponse.ToLower().Trim() == definition.ToLower().Trim();
        }
    }
}
