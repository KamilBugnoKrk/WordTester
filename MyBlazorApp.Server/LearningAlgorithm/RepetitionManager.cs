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
using MyBlazorApp.Server.Helpers;
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
            string pronunciation,
            IEnumerable<string> Responses,
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
                    null,
                    null,
                    RepetitionType.FromTranslatedToOriginalOpen
                ),
                RepetitionType.FromOriginalToTranslatedOpen => (
                    wordStats.Word.OriginalWord,
                    wordStats.Word.Pronunciation,
                    null,
                    RepetitionType.FromOriginalToTranslatedOpen
                ),
                RepetitionType.FromExampleToTranslatedOpen => (
                    wordStats.Word.ExampleUse,
                    wordStats.Word.Pronunciation,
                    null, 
                    RepetitionType.FromExampleToTranslatedOpen
                ),
                RepetitionType.FromDefinitionToOriginalOpen => (
                    wordStats.Word.Definition,
                    null,
                    null,
                    RepetitionType.FromDefinitionToOriginalOpen
                ),
                RepetitionType.FromOriginalToTranslatedClose => (
                    wordStats.Word.OriginalWord,
                    wordStats.Word.Pronunciation,
                    GetOtherTranslatedWords(wordStats.Word.Id, wordStats.Word.CourseId, wordStats.Word.TranslatedWord),
                    RepetitionType.FromOriginalToTranslatedClose
                ),
                RepetitionType.FromTranslatedToOriginalClose => (
                    wordStats.Word.TranslatedWord,
                    null,
                    GetOtherOriginalWords(wordStats.Word.OriginalWord),
                    RepetitionType.FromTranslatedToOriginalClose
                ),
                RepetitionType.FromDefinitionToOriginalClose => (
                    wordStats.Word.Definition,  
                    null,
                    GetOtherOriginalWords(wordStats.Word.OriginalWord),
                    RepetitionType.FromDefinitionToOriginalClose
                ),
                RepetitionType.FromExampleToTranslatedClose => (
                    Helper.ApplyStyleToText(wordStats.Word.ExampleUse),
                    wordStats.Word.Pronunciation,
                    GetOtherTranslatedWords(wordStats.Word.Id, wordStats.Word.CourseId, wordStats.Word.TranslatedWord),
                    RepetitionType.FromExampleToTranslatedClose
                ),
                RepetitionType.FromOriginalToDefinitionClose => (
                   wordStats.Word.OriginalWord,
                   wordStats.Word.Pronunciation,
                   GetOtherDefinitions(wordStats.Word.Id, wordStats.Word.CourseId, wordStats.Word.Definition),
                   RepetitionType.FromOriginalToDefinitionClose
               ),
                RepetitionType.FromExampleToDefinitionClose => (
                   Helper.ApplyStyleToText(wordStats.Word.ExampleUse),
                   wordStats.Word.Pronunciation,
                   GetOtherDefinitions(wordStats.Word.Id, wordStats.Word.CourseId, wordStats.Word.Definition),
                   RepetitionType.FromExampleToDefinitionClose
               ),
                RepetitionType.FromExampleToOriginalClose => (
                 Helper.HideOriginal(wordStats.Word.ExampleUse),
                 null,
                 GetOtherOriginalWords(Helper.GetOriginalFromExample(wordStats.Word.ExampleUse)),
                 RepetitionType.FromExampleToOriginalClose
             ),
                _ => (null, null, null, RepetitionType.None),
            };
        }

        private IEnumerable<string> GetOtherDefinitions(int wordId, int courseId, string definition)
        {
            var newList = _unitOfWork
              .Words
              .GetAll()
              .Where(w => w.CourseId == courseId && w.Id != wordId)
              .ToList()
              .Shuffle()
              .Take(_numberOfIncorrectWords)
              .Select(w => w.Definition)
              .ToList();

            newList.Add(definition);
            return newList.Shuffle();
        }

        private IEnumerable<string> GetOtherOriginalWords(string originalWord)
        {
            var otherWords = new HashSet<string>();

            otherWords.UnionWith(_wordGenerator.GenerateWordsWithoutDoubledLetters(originalWord));
            otherWords.UnionWith(_wordGenerator.GenerateWordsWithSwappedLetters(originalWord));
            
            var newList = otherWords.ToList().Shuffle().Take(_numberOfIncorrectWords).ToList();
            newList.Add(originalWord);
            return newList.Shuffle();
        }

        private IEnumerable<string> GetOtherTranslatedWords(int wordId, int courseId, string correctWord)
        {           
            var newList = _unitOfWork
                .Words
                .GetAll()
                .Where(w => w.CourseId == courseId && w.Id != wordId)
                .ToList()
                .Shuffle()
                .Take(_numberOfIncorrectWords)
                .Select(w => w.TranslatedWord)
                .ToList();

            newList.Add(correctWord);
            return newList.Shuffle();
        }
    }
}
