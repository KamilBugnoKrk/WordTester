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
using Moq;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.LearningAlgorithm;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using System;
using System.Linq;
using Xunit;

namespace MyBlazorApp.Tests.LearningAlgorithm
{
    public class RepetitionManagerTests
    {
        [Fact]
        public void RepetitionManager_OnlyOneRepetitionTypeRemains_ReturnsCorrectRepetitionData()
        {
            var repetitionManager = new RepetitionManager(new Mock<IUnitOfWork>().Object, 
                new Mock<IWordGenerator>().Object);
            
            var result = repetitionManager.CreateRepetitionData(new WordStats 
            { 
                Word = new Word
                {
                    TranslatedWord = "TranslatedWord",
                    OriginalWord = "OriginalWord"
                },
                UsedRepetitionTypes = "[1,3,4,5,6,7,8]"
            });
            
            result.question.Should().Be("TranslatedWord");
            result.Responses.Should().BeNull();
            result.repetitionType.Should().Be(RepetitionType.FromTranslatedToOriginalOpen);
        }
    }
}
