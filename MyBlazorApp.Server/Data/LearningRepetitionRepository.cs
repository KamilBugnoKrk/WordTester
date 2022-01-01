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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace MyBlazorApp.Server.Data
{
    public class LearningRepetitionRepository : Repository<WordStats>, ILearningRepetitionRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public LearningRepetitionRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IEnumerable<WordStats> GetWordStatsByCourseId(int courseId, string userId)
        {
            return _applicationDbContext
                .WordStats
                .Include(ws => ws.Word)
                .Include(ws => ws.Word.Course)
                .Include(ws => ws.Word.Course.UserCourses)
                .Where(ws => (ws.Word.Course.IsVisibleForEveryone ||
                        ws.Word.Course.UserCourses
                            .Any(uc => uc.UserId.ToString() == userId)) &&
                        ws.UserId.ToString() == userId && 
                        ws.Word.Course.Id.ToString() == courseId.ToString());
        }
    }
}
