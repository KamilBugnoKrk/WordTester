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
using System.Collections.Generic;
using System.Linq;

namespace MyBlazorApp.Server.Data
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CourseRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public Course GetUserCourseWithWordsById(int courseId, string userId)
        {
            return _applicationDbContext
                .Courses
                .Include(c => c.Words)
                .Include(c => c.UserCourses)
                .FirstOrDefault(c => !c.IsVisibleForEveryone &&
                        c.UserCourses
                            .Where(uc => uc.UserId.ToString() == userId)
                            .Any() &&
                        c.Id.ToString() == courseId.ToString());
        }

        public IEnumerable<Course> GetMyCoursesWithWords(string userId)
        {
            return _applicationDbContext
                 .Courses
                 .Include(c => c.Words)
                 .Include(c => c.UserCourses)
                 .Where(c => c.IsVisibleForEveryone ||
                         c.UserCourses
                             .Where(uc => uc.UserId.ToString() == userId)
                             .Any())
                 .AsEnumerable();
        }
    }
}
