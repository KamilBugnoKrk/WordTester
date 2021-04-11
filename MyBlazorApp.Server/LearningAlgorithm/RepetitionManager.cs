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

using MyBlazorApp.Server.Data;
using MyBlazorApp.Server.Models;
using MyBlazorApp.Shared;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyBlazorApp.Server.LearningAlgorithm
{
    public class RepetitionManager : IRepetitionManager
    {
        private const int _numberOfIncorrectWords = 5;
        private readonly IEnumerable<RepetitionType> _allRepetitionTypes;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Random _random;
        private readonly IWordGenerator _wordGenerator;

        public RepetitionManager(IUnitOfWork unitOfWork, IWordGenerator wordGenerator)
        {
            _allRepetitionTypes = Enum
                .GetValues(typeof(RepetitionType))
                .Cast<RepetitionType>()
                .Where(rt => rt != RepetitionType.None);
            _random = new Random();
            _unitOfWork = unitOfWork;
            _wordGenerator = wordGenerator;
        }

        public (string question,
            string correctResponse,
            IEnumerable<string> incorrectResponses,
            RepetitionType repetitionType) CreateRepetitionData(WordStats wordStats)
        {

            var availableTypes = _allRepetitionTypes
                .Except(string.IsNullOrEmpty(wordStats.UsedRepetitionTypes) ?
                    new List<RepetitionType>() :
                    JsonConvert.DeserializeObject<List<RepetitionType>>(wordStats.UsedRepetitionTypes))
                .ToList();

            var chosenType = availableTypes.Any() ?
                availableTypes[_random.Next(availableTypes.Count)] :
                _allRepetitionTypes.ToList()[_random.Next(_allRepetitionTypes.Count())];

            return chosenType switch
            {
                RepetitionType.FromTranslatedToOriginalOpen => (
                    wordStats.Word.TranslatedWord,
                    wordStats.Word.OriginalWord,
                    null,
                    RepetitionType.FromTranslatedToOriginalOpen
                ),
                RepetitionType.FromOriginalToTranslatedOpen => (
                    wordStats.Word.OriginalWord,
                    wordStats.Word.TranslatedWord,
                    null,
                    RepetitionType.FromOriginalToTranslatedOpen
                ),
                RepetitionType.FromExampleToTranslatedOpen => (
                    wordStats.Word.ExampleUse,
                    wordStats.Word.TranslatedWord,
                    null, 
                    RepetitionType.FromExampleToTranslatedOpen
                ),
                RepetitionType.FromDefinitionToOriginalOpen => (
                    wordStats.Word.Definition,
                    wordStats.Word.OriginalWord,
                    null,
                    RepetitionType.FromDefinitionToOriginalOpen
                ),
                RepetitionType.FromOriginalToTranslatedClose => (
                    wordStats.Word.OriginalWord,
                    wordStats.Word.TranslatedWord,
                    GetOtherTranslatedWords(wordStats.Word.Id, wordStats.Word.CourseId),
                    RepetitionType.FromOriginalToTranslatedClose
                ),
                RepetitionType.FromTranslatedToOriginalClose => (
                    wordStats.Word.TranslatedWord,
                    wordStats.Word.OriginalWord,
                    GetOtherOriginalWords(wordStats.Word.OriginalWord),
                    RepetitionType.FromTranslatedToOriginalClose
                ),
                RepetitionType.FromDefinitionToOriginalClose => (
                    wordStats.Word.Definition,
                    wordStats.Word.OriginalWord,
                    GetOtherOriginalWords(wordStats.Word.OriginalWord),
                    RepetitionType.FromDefinitionToOriginalClose
                ),
                RepetitionType.FromExampleToTranslatedClose => (
                    wordStats.Word.ExampleUse,
                    wordStats.Word.TranslatedWord,
                    GetOtherTranslatedWords(wordStats.Word.Id, wordStats.Word.CourseId),
                    RepetitionType.FromExampleToTranslatedClose
                ),
                _ => (null, null, null, RepetitionType.None),
            };
        }

        private IEnumerable<string> GetOtherOriginalWords(string originalWord)
        {
            var otherWords = new HashSet<string>();

            otherWords.UnionWith(_wordGenerator.GenerateWordsWithoutDoubledLetters(originalWord));
            otherWords.UnionWith(_wordGenerator.GenerateWordsWithSwappedLetters(originalWord));
            
            return otherWords.ToList().Shuffle().Take(_numberOfIncorrectWords);
        }

        private IEnumerable<string> GetOtherTranslatedWords(int wordId, int courseId)
        {           
            return _unitOfWork
                .Words
                .GetAll()
                .Where(w => w.CourseId == courseId && w.Id != wordId)
                .ToList()
                .Shuffle()
                .Take(_numberOfIncorrectWords)
                .Select(w => w.TranslatedWord);
        }
    }
}
