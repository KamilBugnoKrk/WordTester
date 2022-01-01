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
using System.ComponentModel.DataAnnotations;

namespace MyBlazorApp.Shared
{
    public class WordDto
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(70, ErrorMessage = "Original word should be shorter")]
        public string OriginalWord { get; set; }
        [Required]
        [MaxLength(70, ErrorMessage = "Translated word should be shorter")]
        public string TranslatedWord { get; set; }
        [Required]
        [MaxLength(170, ErrorMessage = "Definition should be shorter")]
        public string Definition { get; set; }
        [Required]
        [MaxLength(170, ErrorMessage = "Example of use should be shorter")]
        [ExampleUse(ErrorMessage = "Example of use should contain *your word*")]
        public string ExampleUse { get; set; }
        [MaxLength(70, ErrorMessage = "Pronunciation should be shorter")]
        public string Pronunciation { get; set; }
    }
}