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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBlazorApp.Server.Data
{
    public class WordRepository : Repository<Word>, IWordRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public WordRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Word GetUserWordById(int wordId, string userId)
        {
            return _applicationDbContext
                .Words
                .Include(w => w.Course)
                .Include(w => w.Course.UserCourses)
                .FirstOrDefault(w => !w.Course.IsVisibleForEveryone &&
                        w.Course.UserCourses
                            .Any(uc => uc.UserId.ToString() == userId) &&
                        w.Id.ToString() == wordId.ToString());
        }

        public bool IsMyWord(string wordId, string userId)
        {
            return _applicationDbContext
                 .Words
                 .Include(w => w.Course)
                 .Include(w => w.Course.UserCourses)
                 .Where(w => (w.Course.IsVisibleForEveryone ||
                         w.Course.UserCourses
                             .Any(uc => uc.UserId.ToString() == userId)) &&
                             w.Id.ToString() == wordId)
                 .Any();
        }

        public Word GetWordWithStatsById(int wordId)
        {
            return _applicationDbContext
                 .Words
                 .Include(w => w.WordStats)
                 .Where(w => w.Id == wordId).FirstOrDefault();                 
        }
    }
}
