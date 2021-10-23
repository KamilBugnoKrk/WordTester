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
using MyBlazorApp.Server.Helpers;
using Xunit;

namespace MyBlazorApp.UiTests
{
    public class HelperTests
    {
        [Theory]
        [InlineData("","")]
        [InlineData("This cat is really fun", "This cat is really fun")]
        [InlineData("This *cat* is really fun", "This CAT is really fun")]
        [InlineData("*This* cat is really fun", "THIS cat is really fun")]
        [InlineData("This cat is really *fun*", "This cat is really FUN")]
        [InlineData("This cat *is really* fun", "This cat IS REALLY fun")]
        public void ApplyStyleToText_Verify(string example, string expectedResult)
        {
            var result = Helper.ApplyStyleToText(example);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("This cat is really fun", "")]
        [InlineData("This *cat* is really fun", "cat")]
        [InlineData("*This* cat is really fun", "this")]
        [InlineData("This cat is really *fun*", "fun")]
        [InlineData("This cat *is really* fun", "is really")]
        public void GetOriginalFromExample_Verify(string example, string expectedResult)
        {
            var result = Helper.GetOriginalFromExample(example);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("This cat is really fun", "This cat is really fun")]
        [InlineData("This *cat* is really fun", "This _____ is really fun")]
        [InlineData("*This* cat is really fun", "_____ cat is really fun")]
        [InlineData("This cat is really *fun*", "This cat is really _____")]
        [InlineData("This cat *is really* fun", "This cat _____ fun")]
        public void HideOriginal_Verify(string example, string expectedResult)
        {
            var result = Helper.HideOriginal(example);
            result.Should().Be(expectedResult);
        }

        [Theory]
        [InlineData("big, great, huge", "big, great, huge", true)]
        [InlineData("big (sth)", "big (sth)", true)]
        [InlineData("big (sth)", "big", true)]
        [InlineData("big (sth)", "big sth", true)]
        [InlineData("big (a), huge (b), great (c)", "big", true)]
        [InlineData("big (a), huge (b), great (c)", "huge", true)]
        [InlineData("big (a), huge (b), great (c)", "great", true)]
        [InlineData("big (a), huge (b),    great (c)", "GREAT", true)]
        [InlineData("big, great, huge", " big, great, huge ", true)]
        [InlineData("big, great, huge", "big", true)]
        [InlineData("big, great, huge", " big ", true)]
        [InlineData("big, great, huge", "great", true)]
        [InlineData("big, great, huge", " great ", true)]
        [InlineData("big, great, huge", "huge", true)]
        [InlineData("  big  ,  great  ,  huge  ", " huge ", true)]
        [InlineData("  big  ,  great  ,  huge  ", " Huge ", true)]
        [InlineData("  big  ,  great (anything)  ,  huge  ", " great ", true)]
        [InlineData("  big  ,  great (anything)  ,  huge  ", " great (anything) ", true)]
        [InlineData("big, great, huge", "other", false)]
        public void IsCorrectTranslation_Verify(string translation, string userAnswer, bool expectedResult)
        {
            var result = Helper.IsCorrectTranslation(translation, userAnswer);
            result.Should().Be(expectedResult);
        }
    }
}
