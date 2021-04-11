﻿// Copyright (C) 2021  Kamil Bugno
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
using System;

namespace MyBlazorApp.Server.Data
{
    public interface IUnitOfWork : IDisposable
    {
        ICourseRepository Courses { get; }
        IWordRepository Words { get; }
        ILearningRepetitionRepository WordStats { get; }
        int Complete();
    }
}
