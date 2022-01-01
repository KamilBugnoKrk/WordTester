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
using Moq;
using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Data.Audio;
using MyBlazorApp.Server.LearningAlgorithm;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using System;
using Xunit;

namespace MyBlazorApp.Tests.LearningAlgorithm
{
    public class RepetitionManagerTests
    {
        [Fact]
        public void RepetitionManager_OnlyOneRepetitionTypeRemains_ReturnsCorrectRepetitionData()
        {
            var repetitionManager = new RepetitionManager(new Mock<IUnitOfWork>().Object, 
                new Mock<IWordGenerator>().Object, new Mock<IAudioService>().Object);
                            
            var (question, _, responses, _, repetitionType) = repetitionManager.CreateRepetitionData(new WordStats 
            { 
                Word = new Word
                {
                    TranslatedWord = "TranslatedWord",
                    OriginalWord = "OriginalWord"
                },
                UsedRepetitionTypes = "[1,3,4,5,6,7,8,9,10,11,12,13]"
            });
            
            question.Should().Be("TranslatedWord");
            responses.Should().BeNull();
            repetitionType.Should().Be(RepetitionType.FromTranslatedToOriginalOpen);
        }

        [Theory]
        [InlineData(0, "OriginalWord", false)]
        [InlineData(1, "TranslatedWord", false)]
        [InlineData(2, "Definition", false)]
        [InlineData(3, "ExampleUse", false)]
        [InlineData(5, "TranslatedWord", true)]
        [InlineData(6, "Definition", true)]
        [InlineData(10, "ExampleUse", true)]
        public void RepetitionManager_AllRepetitionTypeRemains_ReturnsCorrectRepetitionData(int type, string expectedQuestion, bool hasResponses)
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(type);
            var repetitionManager = new RepetitionManager(new Mock<IUnitOfWork>().Object,
                new Mock<IWordGenerator>().Object, new Mock<IAudioService>().Object, randomMock.Object);

            var (question, _, responses, _, repetitionType) = repetitionManager.CreateRepetitionData(new WordStats
            {
                Word = new Word
                {
                    TranslatedWord = "TranslatedWord",
                    OriginalWord = "OriginalWord",
                    Definition = "Definition",
                    ExampleUse = "ExampleUse"                    
                },
                UsedRepetitionTypes = string.Empty
            });

            if (hasResponses)
            {
                responses.Should().NotBeEmpty();
            }
            else
            {
                responses.Should().BeNullOrEmpty();
            }
            question.Should().Be(expectedQuestion);
            ((int)repetitionType).Should().Be(type + 1);
        }
    }
}
