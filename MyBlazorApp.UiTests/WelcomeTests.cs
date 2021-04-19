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

using Bunit;
using FluentAssertions;
using MyBlazorApp.Client.Pages;
using System.Linq;
using Xunit;

namespace MyBlazorApp.UiTests
{
    public class WelcomeTests : TestContext
    {
        [Fact]
        public void HasCorrectLinks()
        {
            var cut = RenderComponent<Welcome>();
            var links = cut.FindAll("a");
            var hrefs = links
                .SelectMany(l => l.Attributes
                    .Where(a => a.Name == "href")
                    .Select(a => a.Value));

            links.Count.Should().Be(3);
            hrefs
                .Should()
                .Contain(new string[] { "/login", "/register", "/about"});
        }
    }
}
