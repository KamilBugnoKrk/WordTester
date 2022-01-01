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
    public class CourseRepositoryTests
    {
        [Fact]
        public void GetCourseWithWordsById_SeveralCourses_ReturnsCorrectCourse()
        {
            var userId = Guid.NewGuid();
            var courseWithCurrentUser = CreateCourse(1, userId);
            var courseWithAnotherUser = CreateCourse(1, Guid.NewGuid());
            var defaultCourse = new Course()
            {
                Id = 1,
                IsVisibleForEveryone = true
            };
            var testList = new List<Course>() 
            { 
                courseWithCurrentUser,
                courseWithAnotherUser,
                defaultCourse
            };
            var context = testList.AsDbSet().MockContext(x => x.Courses);
            var repository = new CourseRepository(context);

            var result = repository.GetUserCourseWithWordsById(1, userId.ToString());

            result.Should().BeEquivalentTo(courseWithCurrentUser);
        }

        [Fact]
        public void GetCourseWithWordsById_NoCourseWithThatId_ReturnsNull()
        {
            var userId = Guid.NewGuid();
            var testList = new List<Course>()
            {
               CreateCourse(1, userId),
               CreateCourse(2, userId)
            };
            var context = testList.AsDbSet().MockContext(x => x.Courses);
            var repository = new CourseRepository(context);

            var result = repository.GetUserCourseWithWordsById(100, userId.ToString());

            result.Should().BeNull();
        }

        [Fact]
        public void GetMyCoursesWithWords_AllTypes_ReturnsDefaultCoursesAndUserCourses()
        {
            var userId = Guid.NewGuid();
            var testList = new List<Course>()
            {
               new Course()
               {
                   Id = 1,
                   IsVisibleForEveryone = true
               },
               CreateCourse(2, Guid.NewGuid()),
               CreateCourse(3, userId)
            };
            var context = testList.AsDbSet().MockContext(x => x.Courses);
            var repository = new CourseRepository(context);

            var result = repository.GetMyCoursesWithWords(userId.ToString());

            result.Count().Should().Be(2);
            result.Should().OnlyContain(c => c.Id == 1 || c.Id == 3);
        }

        private static Course CreateCourse(int courseId, Guid userId)
        {
            return new Course()
            {
                Id = courseId,
                IsVisibleForEveryone = false,
                UserCourses = new List<UserCourse>
                {
                    new UserCourse
                    {
                        UserId = userId
                    }
                }
            };
        }
    }    
}
