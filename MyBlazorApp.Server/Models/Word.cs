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

using System.Collections.Generic;

namespace MyBlazorApp.Server.Models
{
    public record Word
    {
        public int Id { get; set; }
        public string OriginalWord { get; set; }
        public string TranslatedWord { get; set; }
        public string Definition { get; set; }
        public string ExampleUse { get; set; }
        public string Pronunciation { get; set; }
        public bool HasAudioGenerated { get; set; }
        public IEnumerable<WordStats> WordStats { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}