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
using MyBlazorApp.Client.Shared;
using Xunit;

namespace MyBlazorApp.UiTests
{
    public class HeperTests
    {
        [Theory]
        [InlineData("","")]
        [InlineData("This cat is really fun", "This cat is really fun")]
        [InlineData("This *cat* is really fun", "This CAT is really fun")]
        [InlineData("*This* cat is really fun", "THIS cat is really fun")]
        [InlineData("This cat is really *fun*", "This cat is really FUN")]
        [InlineData("This cat *is really* fun", "This cat IS REALLY fun")]
        public void HasCorrectLinks(string example, string expectedResult)
        {
            var result = Helper.ApplyStyleToText(example);
            result.Should().Be(expectedResult);
        }
    }
}
