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

using MyBlazorApp.Server.Data;
using System.Collections.Generic;

namespace MyBlazorApp.Server.Models
{
    public record Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsVisibleForEveryone { get; set; }
        public int? LanguageId { get; set; }
        public Language Language { get; set; }
        public IEnumerable<Word> Words { get; set; }
        public IEnumerable<UserCourse> UserCourses { get; set; }
    }
}
