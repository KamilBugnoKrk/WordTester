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

using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Server.Data;
using Xunit;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared.RequestModels;
using System;
using FluentAssertions;
using System.Collections.Generic;
using MyBlazorApp.Tests.Utils;
using System.Threading.Tasks;
using Moq;
using MyBlazorApp.Server.Data.Audio;

namespace MyBlazorApp.Tests
{
    public class DeleteWordCommandHandlerTests
    {
        [Fact]
        public async Task DeleteWordCommandHandler_DeleteExistingWord_WordDeleted()
        {
            using var context = TestHelper.CreateInMemoryContext("DeleteWordCommandHandler_DeleteExistingWord_WordDeleted");
            var (request, handler) = PrepareData(context, "3");

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeTrue();
            (await context.Words.CountAsync()).Should().Be(2);
            (await context.Words
                .FirstOrDefaultAsync(w => w.OriginalWord == "OriginalWord3"))
                .Should().BeNull();
        }

        [Fact]
        public async Task DeleteWordCommandHandler_DeleteCommonCourseWord_WordNotDeleted()
        {
            using var context = TestHelper.CreateInMemoryContext("DeleteWordCommandHandler_DeleteCommonCourseWord_WordNotDeleted");
            var (request, handler) = PrepareData(context, "2");

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeFalse();
            (await context.Words.CountAsync()).Should().Be(3);
            (await context.Words
                .FirstOrDefaultAsync(w => w.OriginalWord == "OriginalWord2"))
                .Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteWordCommandHandler_DeleteDifferentUserWord_WordNotDeleted()
        {
            using var context = TestHelper.CreateInMemoryContext("DeleteWordCommandHandler_DeleteDifferentUserWord_WordNotDeleted");
            var (request, handler) = PrepareData(context, "1");

            var response = await handler.Handle(request, CancellationToken.None);

            response.IsSucceed.Should().BeFalse();
            (await context.Words.CountAsync()).Should().Be(3);
            (await context.Words
                .FirstOrDefaultAsync(w => w.OriginalWord == "OriginalWord1"))
                .Should().NotBeNull();
        }

        private static (DeleteWordRequestModel request, DeleteWordCommandHandler handler) PrepareData(ApplicationDbContext context, string wordId)
        {
            var userId = Guid.NewGuid();
            SeedInMemoryDatabase(context, userId);
            var handler = new DeleteWordCommandHandler(new UnitOfWork(context), new Mock<IAudioService>().Object);
            var request = new DeleteWordRequestModel
            {
                UserId = userId.ToString(),
                WordId = wordId
            };
            return (request, handler);
        }

        private static void SeedInMemoryDatabase(ApplicationDbContext context, Guid userId)
        {
            var differentUser = Guid.NewGuid();
            context.Words.Add(new Word
            {
                Id = 1,
                OriginalWord = "OriginalWord1",
                TranslatedWord = "TranslatedWord1",
                Definition = "Definition1",
                ExampleUse = "ExampleUse1",
                Pronunciation = "Pronunciation1",
                HasAudioGenerated = true,
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
                OriginalWord = "OriginalWord2",
                TranslatedWord = "TranslatedWord2",
                Definition = "Definition2",
                ExampleUse = "ExampleUse2",
                Pronunciation = "Pronunciation2",
                Course = new Course
                {
                    Id = 20,
                    IsVisibleForEveryone = true,                    
                }
            });

            context.Words.Add(new Word
            {
                Id = 3,
                OriginalWord = "OriginalWord3",
                TranslatedWord = "TranslatedWord3",
                Definition = "Definition3",
                ExampleUse = "ExampleUse3",
                Pronunciation = "Pronunciation3",
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

