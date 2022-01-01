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

using System.Collections.Generic;

namespace MyBlazorApp.Shared.ResponseModels
{
    public class GetCourseUserStatsResponseModel
    {
        public Dictionary<string, StatsForThisCourse> StatsForCourses { get; set; }
    }

    public record StatsForThisCourse
    {
        public Dictionary<string, int> MonthStats { get; set; }
        public string FirstRepetitionDate { get; set; }
        public int AllRepetitions { get; set; }
        public (long NumberOfCorrectResponses, long NumberOfIncorrectResponses) LastThreeDays { get; set; }
        public Dictionary<int, int> RepetitionStats { get; set; }
        public int NewWords { get; set; }
    }
}