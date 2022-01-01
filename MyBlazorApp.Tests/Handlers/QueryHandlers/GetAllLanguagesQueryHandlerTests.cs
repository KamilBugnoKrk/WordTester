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

using MyBlazorApp.Server.Data;
using Xunit;
using MyBlazorApp.Server.Handlers.QueryHandlers;
using System.Threading;
using MyBlazorApp.Shared.RequestModels;
using FluentAssertions;
using MyBlazorApp.Tests.Utils;
using System.Threading.Tasks;

namespace MyBlazorApp.Tests
{
    public class GetAllLanguagesQueryHandlerTests
    {
        [Fact]
        public async Task GetAllLanguagesQueryHandler_GetLanguage_ReturnLanguageName()
        {
            using var context = TestHelper.CreateInMemoryContext("GetAllLanguagesQueryHandler_GetLanguage_ReturnLanguageName");
            context.Languages.Add(new Language
            {
                Id = 1,
                Name = "Name1",
                VoiceName = "VoiceName1"
            });
            context.Languages.Add(new Language
            {
                Id = 2,
                Name = "Name2",
                VoiceName = "VoiceName2"
            });
            context.SaveChanges();

            var handler = new GetAllLanguagesQueryHandler(new UnitOfWork(context));
            var request = new GetAllLanguagesRequestModel();

            var response = await handler.Handle(request, CancellationToken.None);

            response.LanguageNames.Should().Contain(new string[]{"Name1", "Name2"});
        }        
    }
}    

