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

using AutoMapper;
using MediatR;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.LearningAlgorithm;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class GetLearningRepetitionQueryHandler : IRequestHandler<GetLearningRepetitionRequestModel, GetLearningRepetitionResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRepetitionManager _repetitionManager;
        public GetLearningRepetitionQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRepetitionManager repetitionManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _repetitionManager = repetitionManager;
        }
        public Task<GetLearningRepetitionResponseModel> Handle(GetLearningRepetitionRequestModel request, CancellationToken cancellationToken)
        {
            if (!IsUserOrDefaultCourse(request.UserId, request.CourseId))
                return CreateErrorResponse();

            var wordStats = _unitOfWork
                .WordStats?
                .GetWordStatsByCourseId(request.CourseId, request.UserId);

            return wordStats == null ?
                LearnNewWord(request) :
                ContinueLearning(request, wordStats);
        }

        private bool IsUserOrDefaultCourse(string userId, int courseId)
        {
            var userOrDefaultCourses = _unitOfWork
                .Courses
                .GetMyCoursesWithWords(userId)
                .Select(course => course.Id);

            if (!userOrDefaultCourses.Any())
                return false;

            return userOrDefaultCourses.Contains(courseId);
        }

        private static Task<GetLearningRepetitionResponseModel> CreateErrorResponse()
        {
            var response = new GetLearningRepetitionResponseModel
            {
                ResponseType = ResponseType.ErrorResponse
            };
            return Task.FromResult(response);
        }

        private Task<GetLearningRepetitionResponseModel> ContinueLearning(GetLearningRepetitionRequestModel request, IEnumerable<Models.WordStats> wordStats)
        {
            var wordToRepeat = wordStats
                .ToList()
                .Shuffle()
                .FirstOrDefault(ws => ws.NextRevisionTime <= DateTime.Now);

            return wordToRepeat == null ? 
                LearnNewWord(request) : 
                RepeatLearnedWord(wordToRepeat);
        }

        private Task<GetLearningRepetitionResponseModel> LearnNewWord(GetLearningRepetitionRequestModel request)
        {
            //learn a first word
            var notLearnedWord = _unitOfWork
                    .Words
                    .GetAll()
                    .Where(w => w.CourseId == request.CourseId && w.WordStats == null)
                    .ToList()
                    .Shuffle()
                    .FirstOrDefault();

            return notLearnedWord == null ?
               CreateNothingToLearnResponse() :
               CreateInformationalResponse(notLearnedWord);
        }

        private Task<GetLearningRepetitionResponseModel> CreateInformationalResponse(Word notLearnedWord)
        {
            var response = new GetLearningRepetitionResponseModel
            {
                WordId = notLearnedWord.Id,
                ResponseType = ResponseType.InformationalResponse,
                InformationalWord = _mapper.Map<Word, WordDto>(notLearnedWord)                
            };
            return Task.FromResult(response);
        }

        private Task<GetLearningRepetitionResponseModel> RepeatLearnedWord(WordStats wordStats)
        {
            var (question, pronunciation, responses, repetitionType) = _repetitionManager
                .CreateRepetitionData(wordStats);

            var response = new GetLearningRepetitionResponseModel
            {
                WordStatsId = wordStats.Id,
                WordId = wordStats.WordId,
                ResponseType = ResponseType.PracticeResponse,
                Question = question,
                Responses = responses,
                RepetitionType = repetitionType,
                Pronunciation = pronunciation
            };

            return Task.FromResult(response);
        }

        private Task<GetLearningRepetitionResponseModel> CreateNothingToLearnResponse()
        {
            var response = new GetLearningRepetitionResponseModel
            {
                ResponseType = ResponseType.NothingToLearnResponse
            };
            return Task.FromResult(response);
        }
    }
}
