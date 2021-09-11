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

using MyBlazorApp.Server.Models;

namespace MyBlazorApp.Server.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Courses = new CourseRepository(_context);
            Words = new WordRepository(_context);
            WordStats = new LearningRepetitionRepository(_context);
            Languages = new Repository<Language>(_context);
            UserCourseStats = new Repository<UserCourseStats>(_context);
        }

        public ICourseRepository Courses { get; }

        public IWordRepository Words { get; }
        public ILearningRepetitionRepository WordStats { get; }
        public IRepository<Language> Languages { get; set; }
        public IRepository<UserCourseStats> UserCourseStats { get; set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
