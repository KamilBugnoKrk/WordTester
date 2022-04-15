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

using Combinatorics.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyBlazorApp.Server.LearningAlgorithm
{
    public class WordGenerator : IWordGenerator
    {
        private readonly Random _random;

        public WordGenerator(Random random = null)
        {
            _random = random ?? new Random();
        }

        public IEnumerable<string> GenerateWordsWithSwappedLetters(string originalWord)
        {
            var otherWords = new HashSet<string>();

            AddWordVariations(originalWord, otherWords, new List<string> { "a", "e", "i", "o", "u" });
            AddWordVariations(originalWord, otherWords, new List<string> { "p", "b", "d" });
            AddWordVariations(originalWord, otherWords, new List<string> { "q", "g" });
            AddWordVariations(originalWord, otherWords, new List<string> { "v", "w" });
            AddWordVariations(originalWord, otherWords, new List<string> { "l", "r" });
            AddWordVariations(originalWord, otherWords, new List<string> { "п", "л", "д" });
            AddWordVariations(originalWord, otherWords, new List<string> { "ь", "ъ", "ы", "б" });
            AddWordVariations(originalWord, otherWords, new List<string> { "щ", "ш" });

            return otherWords;
        }

        public IEnumerable<string> GenerateWordsWithDifferentPrepositions(string originalWord)
        {
            var otherWords = new HashSet<string>();

            AddWordWithDifferentPrepositions(originalWord, otherWords, new List<string> { "onto", "on", "into", "in", "off", "up", "out", "for", "from", "of", "at" });
            AddWordWithDifferentPrepositions(originalWord, otherWords, new List<string> { "around", "among", "along", "against", "across", "above" });
            AddWordWithDifferentPrepositions(originalWord, otherWords, new List<string> { "to", "with", "towards", "through", "throughout" });
            AddWordWithDifferentPrepositions(originalWord, otherWords, new List<string> { "beyond", "behind", "between", "before"});

            return otherWords;
        }

        private void AddWordWithDifferentPrepositions(string originalWord, HashSet<string> otherWords, List<string> newPrepositions)
        {
            var variations = new Variations<string>(newPrepositions, 2, GenerateOption.WithoutRepetition);
            foreach (var variation in variations)
            {
                if (originalWord.Contains($" {variation[0]} ") || originalWord.EndsWith($" {variation[0]}"))
                {
                    string pattern = $@"\b{variation[0]}\b";
                    string result = Regex.Replace(originalWord, pattern, variation[1]);
                    otherWords.Add(result);
                }
            }
        }

        private void AddWordVariations(string originalWord, HashSet<string> otherWords, List<string> letters)
        {
            var variations = new Variations<string>(letters, 2, GenerateOption.WithoutRepetition);
            foreach (var variation in variations)
            {
                if (originalWord.Contains(variation[0]))
                    otherWords.Add(ReplaceOccurrence(originalWord, variation[0], variation[1]));
            }
        }

        private string ReplaceOccurrence(string source, string find, string replace)
        {
            int place = _random.Next(100) < 50 ?
                source.IndexOf(find) :
                source.LastIndexOf(find);

            return source
                .Remove(place, 1)
                .Insert(place, replace);
        }

        public IEnumerable<string> GenerateWordsWithoutDoubledLetters(string originalWord)
        {
            var otherWords = new HashSet<string>();
            var doubledLettersPattern = @"(.)\1+";
            var myRegex = new Regex(doubledLettersPattern, RegexOptions.IgnoreCase);
            var matched = myRegex.Matches(originalWord);

            foreach (var match in matched.ToList())
            {
                otherWords.Add(originalWord.Replace(match.Value, match.Value[0].ToString()));
            }

            return otherWords;
        }
    }
}
