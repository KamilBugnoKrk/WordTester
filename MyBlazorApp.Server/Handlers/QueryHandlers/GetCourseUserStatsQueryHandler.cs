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
using MyBlazorApp.Shared.RequestModels;
using MyBlazorApp.Shared.ResponseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Handlers.QueryHandlers
{
    public class GetCourseUserStatsQueryHandler : IRequestHandler<GetCourseUserStatsRequestModel, GetCourseUserStatsResponseModel>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetCourseUserStatsQueryHandler(IUnitOfWork unitOfWork)
        {           
            _unitOfWork = unitOfWork;
        }
        public Task<GetCourseUserStatsResponseModel> Handle(GetCourseUserStatsRequestModel request, CancellationToken cancellationToken)
        {
            Dictionary<string, StatsForThisCourse> statsForCourses = RetrieveStatsForAllUserCourses(request);

            return Task.FromResult(new GetCourseUserStatsResponseModel
            {
                StatsForCourses = statsForCourses
            });
        }

        private Dictionary<string, StatsForThisCourse> RetrieveStatsForAllUserCourses(GetCourseUserStatsRequestModel request)
        {
            var courseInfos = _unitOfWork.Courses.GetMyCoursesWithWords(request.UserId.ToString()).Select(c => new { c.Name, c.Id }).ToList();
            var data = new Dictionary<string, StatsForThisCourse>();
            CultureInfo cultureInfo = new("en-US");
            foreach (var course in courseInfos)
            {
                var stats = _unitOfWork.UserCourseStats
                    .Find(ucs => ucs.UserId == request.UserId && ucs.CourseId == course.Id);
                if(!stats.Any())
                {
                    data.Add(course.Name, null);
                    continue;
                }
                var monthStats = GetMonthStats(stats);
                var firstRepetitionDate = stats.Min(s => s.Date).ToString("dddd, dd MMMM yyyy", cultureInfo);
                var allRepetitions = stats.Sum(s => s.NumberOfCorrectResponses + s.NumberOfIncorrectResponses);
                var threeDaysIncorrectResponses = stats.Where(s => s.Date >= DateTime.UtcNow.AddDays(-3)).Sum(s => s.NumberOfIncorrectResponses);
                var threeDaysCorrectResponses = stats.Where(s => s.Date >= DateTime.UtcNow.AddDays(-3)).Sum(s => s.NumberOfCorrectResponses);

                var repetitionStats = _unitOfWork.Words.Find(w => w.CourseId == course.Id)
                    .SelectMany(w => w.WordStats)
                    .Where(ws => ws.UserId == request.UserId)
                    .GroupBy(ws => ws.RevisionFactor).Select(group => new
                    {
                        a = (int)group.Key,
                        b = group.Count()
                    }).ToDictionary(o => o.a, o => o.b);

                var allWords = _unitOfWork
                    .Words
                    .Find(w => w.CourseId == course.Id)
                    .Count();

                var repeatedWords = _unitOfWork
                    .Words
                    .Find(w => w.CourseId == course.Id)
                    .Count(w => w.WordStats.Any(ws => ws.UserId == request.UserId));

                data.Add(course.Name, new StatsForThisCourse 
                { 
                    MonthStats = monthStats, 
                    FirstRepetitionDate = firstRepetitionDate, 
                    AllRepetitions = (int)allRepetitions, 
                    LastThreeDays = (threeDaysCorrectResponses, threeDaysIncorrectResponses),
                    RepetitionStats = repetitionStats,
                    NewWords = allWords - repeatedWords
                });
            }

            return data;
        }

        private static Dictionary<string, int> GetMonthStats(IEnumerable<UserCourseStats> stats)
        {
            Dictionary<string, int> monthStats = new();
            for (int i = 0; i > -4; i--)
            {
                var thisMonthResponses = stats
                .Where(s => s.Date.Year == DateTime.UtcNow.AddMonths(i).Year && s.Date.Month == DateTime.UtcNow.AddMonths(i).Month)
                .Sum(s => s.NumberOfCorrectResponses + s.NumberOfIncorrectResponses);
                monthStats.Add(DateTime.UtcNow.AddMonths(i).ToString("MMMM"), (int)thisMonthResponses);
            }
            return monthStats;
        }
    }
}
