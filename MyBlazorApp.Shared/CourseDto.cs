﻿// Copyright (C) 2022  Kamil Bugno
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

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyBlazorApp.Shared
{
    public class CourseDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(30, ErrorMessage = "Title should be shorter")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MaxLength(70, ErrorMessage = "Description should be shorter")]
        public string Description { get; set; }
        public bool IsVisibleForEveryone { get; set; }
        public int NumberOfWords { get; set; }
        [Required(ErrorMessage = "Language name is required")]
        public string SelectedLanguageName { get; set; }
        public List<string> LanguageOptions { get; set; }
        public int NumberOfKnownWords { get; set; }
        public Dictionary<long, int> Stats { get; set; }
        public Dictionary<DateTime, int> NumberOfCorrectRepetitions { get; set; }
    }
}
