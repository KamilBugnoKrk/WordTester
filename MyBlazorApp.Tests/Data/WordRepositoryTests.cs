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

using FluentAssertions;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Tests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MyBlazorApp.Tests
{
    public class WordRepositoryTests
    {
        [Fact]
        public void GetUserWordById_UserCourse_ReturnsCourse()
        {
            var userId = Guid.NewGuid();
            var testList = CreateWords(userId, 10, 20, false);
            var context = testList.AsDbSet().MockContext(x => x.Words);
            var repository = new WordRepository(context);

            var result = repository.GetUserWordById(10, userId.ToString());
            result.Id.Should().Be(10);
        }

        [Fact]
        public void GetUserWordById_DifferentUserCourse_ReturnsNull()
        {
            var testList = CreateWords(Guid.NewGuid(), 10, 20, false);
            var context = testList.AsDbSet().MockContext(x => x.Words);
            var repository = new WordRepository(context);

            var result = repository.GetUserWordById(10, Guid.NewGuid().ToString());
            result.Should().BeNull();
        }

        [Fact]
        public void GetUserWordById_DefaultCourse_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            List<Word> testList = CreateWords(userId, 10, 20, true);
            var context = testList.AsDbSet().MockContext(x => x.Words);
            var repository = new WordRepository(context);

            var result = repository.GetUserWordById(10, userId.ToString());
            result.Should().BeNull();
        }

        [Fact]
        public void GetUserWordById_NoCourse_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            List<Word> testList = CreateWords(userId, 10, 20, true);
            var context = testList.AsDbSet().MockContext(x => x.Words);
            var repository = new WordRepository(context);

            var result = repository.GetUserWordById(30, userId.ToString());
            result.Should().BeNull();
        }

        private static List<Word> CreateWords(Guid userId, int wordId, int courseId, bool isVisibleForEveryone)
        {
            return new List<Word>
            {
                new Word
                {
                    Id = wordId,
                    OriginalWord = "OriginalWord",
                    TranslatedWord = "TranslatedWord",
                    Definition = "Definition",
                    ExampleUse = "ExampleUse",
                    Pronunciation = "Pronunciation",
                    Course = new Course
                    {
                        Id = courseId,
                        IsVisibleForEveryone = isVisibleForEveryone,
                        UserCourses = new List<UserCourse>
                        {
                            new UserCourse
                            {
                                UserId = userId
                            }
                        }
                    }
                }
            };
        }
    }    
}
