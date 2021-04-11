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

using System.ComponentModel.DataAnnotations;

namespace MyBlazorApp.Shared
{
    public class CourseDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Name should be shorter")]
        public string Name { get; set; }
        [Required]
        [MaxLength(70, ErrorMessage = "Description should be shorter")]
        public string Description { get; set; }
        public bool IsVisibleForEveryone { get; set; }
        public int NumberOfWords { get; set; }
    }
}
