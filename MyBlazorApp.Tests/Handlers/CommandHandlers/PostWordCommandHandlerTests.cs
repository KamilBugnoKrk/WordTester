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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Data;
using Xunit;
using Microsoft.EntityFrameworkCore.InMemory;
using MyBlazorApp.Server.Controllers;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Tests.Utils;

namespace MyBlazorApp.Tests
{
    public class PostWordCommandHandlerTests
    {
        [Fact]
        public async void PostWordCommandHandler_AddNewWord_WordAdded()
        {
            using var context = TestHelper.CreateInMemoryContext("PostWordCommandHandler_AddNewWord_WordAdded");
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new PostWordCommandHandler(new UnitOfWork(context));
            var request = new PostWordRequestModel
            {
                UserId = userId.ToString(),
                Word = new Shared.WordDto
                {
                    OriginalWord = "MyOriginalWord"
                },
                CourseId = "30"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeTrue();
            (await context.Words.CountAsync()).Should().Be(4);
        }

        [Fact]
        public async void PostWordCommandHandler_AddNewWordForDefaultCourse_ReturnError()
        {
            using var context = TestHelper.CreateInMemoryContext("PostWordCommandHandler_AddNewWordForDefaultCourse_ReturnError");
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new PostWordCommandHandler(new UnitOfWork(context));
            var request = new PostWordRequestModel
            {
                UserId = userId.ToString(),
                Word = new Shared.WordDto
                {
                    OriginalWord = "MyOriginalWord"
                },
                CourseId = "20"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeFalse();
            response.Error.Should().Be("You can modify only your courses");
        }

        [Fact]
        public async void PostWordCommandHandler_AddNewWordForAnotherUserCourse_ReturnError()
        {
            using var context = TestHelper.CreateInMemoryContext("PostWordCommandHandler_AddNewWordForAnotherUserCourse_ReturnError");
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new PostWordCommandHandler(new UnitOfWork(context));
            var request = new PostWordRequestModel
            {
                UserId = userId.ToString(),
                Word = new Shared.WordDto
                {
                    OriginalWord = "MyOriginalWord"
                },
                CourseId = "10"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeFalse();
            response.Error.Should().Be("You can modify only your courses");
        }

        [Fact]
        public async void PostWordCommandHandler_ModifyWord_WordModified()
        {
            using var context = TestHelper.CreateInMemoryContext("PostWordCommandHandler_ModifyWord_WordModified");
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new PostWordCommandHandler(new UnitOfWork(context));
            var request = new PostWordRequestModel
            {
                UserId = userId.ToString(),
                Word = new Shared.WordDto
                {
                    Id = 3,
                    OriginalWord = "NewOriginalWord",
                    TranslatedWord = "NewTranslatedWord",
                    Definition = "NewDefinition",
                    ExampleUse = "NewExampleUse",
                    Pronunciation = "NewPronunciation",
                },
                CourseId = "30"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeTrue();
            (await context.Words.CountAsync()).Should().Be(3);
            var modifiedWord = context.Words.Find(3);
            modifiedWord.OriginalWord.Should().Be("NewOriginalWord");
            modifiedWord.TranslatedWord.Should().Be("NewTranslatedWord");
            modifiedWord.Definition.Should().Be("NewDefinition");
            modifiedWord.ExampleUse.Should().Be("NewExampleUse");
            modifiedWord.Pronunciation.Should().Be("NewPronunciation");
        }

        [Fact]
        public async void PostWordCommandHandler_ModifyAnotherUserWord_WordModified()
        {
            using var context = TestHelper.CreateInMemoryContext("PostWordCommandHandler_ModifyAnotherUserWord_WordModified");
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new PostWordCommandHandler(new UnitOfWork(context));
            var request = new PostWordRequestModel
            {
                UserId = userId.ToString(),
                Word = new Shared.WordDto
                {
                    Id = 1,
                    OriginalWord = "NewOriginalWord",
                    TranslatedWord = "NewTranslatedWord",
                    Definition = "NewDefinition",
                    ExampleUse = "NewExampleUse",
                    Pronunciation = "NewPronunciation",
                },
                CourseId = "30"
            };

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeFalse();
            response.Error.Should().Be("You can modify only words that belong to your course");
        }

        private static void SeedInMemoryDatabase(ApplicationDbContext context, Guid userId)
        {
            var differentUser = Guid.NewGuid();
            context.Words.Add(new Word
            {
                Id = 1,
                OriginalWord = "OriginalWord",
                TranslatedWord = "TranslatedWord",
                Definition = "Definition",
                ExampleUse = "ExampleUse",
                Pronunciation = "Pronunciation",
                Course = new Course
                {
                    Id = 10,
                    IsVisibleForEveryone = false,
                    UserCourses = new List<UserCourse>
                    {
                        new UserCourse
                        {
                            UserId = differentUser
                        }
                    }
                }
            });

            context.Words.Add(new Word
            {
                Id = 2,
                OriginalWord = "OriginalWord",
                TranslatedWord = "TranslatedWord",
                Definition = "Definition",
                ExampleUse = "ExampleUse",
                Pronunciation = "Pronunciation",
                Course = new Course
                {
                    Id = 20,
                    IsVisibleForEveryone = true,                    
                }
            });

            context.Words.Add(new Word
            {
                Id = 3,
                OriginalWord = "OriginalWord",
                TranslatedWord = "TranslatedWord",
                Definition = "Definition",
                ExampleUse = "ExampleUse",
                Pronunciation = "Pronunciation",
                Course = new Course
                {
                    Id = 30,
                    IsVisibleForEveryone = false,
                    UserCourses = new List<UserCourse>
                    {
                        new UserCourse
                        {
                            UserId = userId
                        }
                    }
                }
            });

            context.SaveChanges();
        }      
    }
}    

