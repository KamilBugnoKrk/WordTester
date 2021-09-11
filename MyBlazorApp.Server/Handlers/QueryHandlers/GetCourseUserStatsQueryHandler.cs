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
            Dictionary<int, StatsForThisCourse> statsForCourses = RetrieveStatsForAllUserCourses(request);

            return Task.FromResult(new GetCourseUserStatsResponseModel
            {
                StatsForCourses = statsForCourses
            });
        }

        private Dictionary<int, StatsForThisCourse> RetrieveStatsForAllUserCourses(GetCourseUserStatsRequestModel request)
        {
            var courseIds = _unitOfWork.Courses.GetMyCoursesWithWords(request.UserId.ToString()).Select(c => c.Id).ToList();
            var data = new Dictionary<int, StatsForThisCourse>();
            foreach (var courseId in courseIds)
            {
                var stats = _unitOfWork.UserCourseStats
                    .Find(ucs => ucs.UserId == request.UserId && ucs.CourseId == courseId);
                var monthStats = GetMonthStats(stats);

                data.Add(courseId, new StatsForThisCourse { MonthStats = monthStats });
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
