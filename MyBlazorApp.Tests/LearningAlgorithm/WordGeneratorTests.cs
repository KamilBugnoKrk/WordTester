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
using MyBlazorApp.Server.LearningAlgorithm;
using System;
using System.Linq;
using Xunit;

namespace MyBlazorApp.Tests.LearningAlgorithm
{
    public class WordGeneratorTests
    {
        [Fact]
        public void GenerateWordsWithoutDoubledLetters_WordContainOneOccurence_ReturnsGeneratedWords()
        {
            var generator = new WordGenerator(null);
            
            var result = generator.GenerateWordsWithoutDoubledLetters("fleer");
            
            result.Should().NotBeEmpty();
            result.First().Should().Be("fler");
        }

        [Fact]
        public void GenerateWordsWithoutDoubledLetters_WordContainTwoOccurences_ReturnsGeneratedWords()
        {
            var generator = new WordGenerator(null);

            var result = generator.GenerateWordsWithoutDoubledLetters("succeed");

            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo("suceed", "succed");
        }

        [Fact]
        public void GenerateWordsWithoutDoubledLetters_WordContainNoOccurences_ReturnsEmpty()
        {
            var generator = new WordGenerator(null);

            var result = generator.GenerateWordsWithoutDoubledLetters("cat");

            result.Should().BeEmpty();
        }

        [Fact]
        public void GenerateWordsWithDifferentPrepositions_WordContainOneOccurences_ReturnsModifiedWords()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
            var generator = new WordGenerator(randomMock.Object);

            var result = generator.GenerateWordsWithDifferentPrepositions("income up");
            result.Should().BeEquivalentTo("income for", "income from", "income of", "income at", "income onto", "income on", "income into", "income in", "income off", "income out");
        }

        [Fact]
        public void GenerateWordsWithSwappedLetters_WordContainOneOccurences_ReturnsModifiedWords()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
            var generator = new WordGenerator(randomMock.Object);

            var result = generator.GenerateWordsWithSwappedLetters("cat");

            result.Should().BeEquivalentTo("cot", "cut", "cit", "cet");
        }

        [Fact]
        public void GenerateWordsWithSwappedLetters_WordContainTwoOccurences_ReturnsModifiedWords()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
            var generator = new WordGenerator(randomMock.Object);

            var result = generator.GenerateWordsWithSwappedLetters("dog");

            result.Should().BeEquivalentTo("pog", "bog", "dig", "dug", "dag", "deg", "doq");
        }

        [Fact]
        public void GenerateWordsWithSwappedLetters_WordContainV_ReturnsModifiedWords()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
            var generator = new WordGenerator(randomMock.Object);

            var result = generator.GenerateWordsWithSwappedLetters("v");

            result.Should().BeEquivalentTo("w");
        }

        [Fact]
        public void GenerateWordsWithSwappedLetters_WordContainSeveralOccurences_ReturnsModifiedWords()
        {
            var randomMock = new Mock<Random>();
            randomMock.Setup(r => r.Next(It.IsAny<int>())).Returns(0);
            var generator = new WordGenerator(randomMock.Object);

            var result = generator.GenerateWordsWithSwappedLetters("catastrophic");

            result.Should().BeEquivalentTo("cetastrophic", "citastrophic", 
                "cotastrophic", "cutastrophic", "catastrophac", "catastrophec",
                "catastrophoc", "catastrophuc", "catastrobhic", "catastrodhic",
                "catastlophic", "catastraphic", "catastrephic", "catastriphic",
                "catastruphic");
        }
    }
}
