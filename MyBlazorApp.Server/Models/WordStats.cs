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

namespace MyBlazorApp.Server.Models
{
    public record WordStats
    {
        public int Id { get; set; }
        public long RevisionFactor { get; set; }
        public string UsedRepetitionTypes { get; set; }
        public long NextRevisionTicks { get; set; }
        public DateTime NextRevisionTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int WordId { get; set; }
        public Word Word { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
