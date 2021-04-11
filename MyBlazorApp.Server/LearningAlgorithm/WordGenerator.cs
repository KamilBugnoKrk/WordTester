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
          
            //todo use permutations
            if (originalWord.Contains("p"))
                otherWords.Add(ReplaceOccurrence(originalWord, "p", "b"));
            if (originalWord.Contains("p"))
                otherWords.Add(ReplaceOccurrence(originalWord, "p", "d"));
            if (originalWord.Contains("b"))
                otherWords.Add(ReplaceOccurrence(originalWord, "b", "p"));
            if (originalWord.Contains("b"))
                otherWords.Add(ReplaceOccurrence(originalWord, "b", "d"));
            if (originalWord.Contains("d"))
                otherWords.Add(ReplaceOccurrence(originalWord, "d", "p"));
            if (originalWord.Contains("d"))
                otherWords.Add(ReplaceOccurrence(originalWord, "d", "b"));

            if (originalWord.Contains("a"))
                otherWords.Add(ReplaceOccurrence(originalWord, "a", "e"));
            if (originalWord.Contains("a"))
                otherWords.Add(ReplaceOccurrence(originalWord, "a", "i"));
            if (originalWord.Contains("a"))
                otherWords.Add(ReplaceOccurrence(originalWord, "a", "o"));
            if (originalWord.Contains("a"))
                otherWords.Add(ReplaceOccurrence(originalWord, "a", "u"));
            if (originalWord.Contains("e"))
                otherWords.Add(ReplaceOccurrence(originalWord, "e", "i"));
            if (originalWord.Contains("e"))
                otherWords.Add(ReplaceOccurrence(originalWord, "e", "a"));
            if (originalWord.Contains("e"))
                otherWords.Add(ReplaceOccurrence(originalWord, "e", "o"));
            if (originalWord.Contains("e"))
                otherWords.Add(ReplaceOccurrence(originalWord, "e", "u"));
            if (originalWord.Contains("i"))
                otherWords.Add(ReplaceOccurrence(originalWord, "i", "a"));
            if (originalWord.Contains("i"))
                otherWords.Add(ReplaceOccurrence(originalWord, "i", "e"));
            if (originalWord.Contains("i"))
                otherWords.Add(ReplaceOccurrence(originalWord, "i", "o"));
            if (originalWord.Contains("i"))
                otherWords.Add(ReplaceOccurrence(originalWord, "i", "u"));
            if (originalWord.Contains("o"))
                otherWords.Add(ReplaceOccurrence(originalWord, "o", "a"));
            if (originalWord.Contains("o"))
                otherWords.Add(ReplaceOccurrence(originalWord, "o", "e"));
            if (originalWord.Contains("o"))
                otherWords.Add(ReplaceOccurrence(originalWord, "o", "i"));
            if (originalWord.Contains("o"))
                otherWords.Add(ReplaceOccurrence(originalWord, "o", "u"));
            if (originalWord.Contains("u"))
                otherWords.Add(ReplaceOccurrence(originalWord, "u", "a"));
            if (originalWord.Contains("u"))
                otherWords.Add(ReplaceOccurrence(originalWord, "u", "e"));
            if (originalWord.Contains("u"))
                otherWords.Add(ReplaceOccurrence(originalWord, "u", "i"));
            if (originalWord.Contains("u"))
                otherWords.Add(ReplaceOccurrence(originalWord, "u", "o"));
            
            if (originalWord.Contains("q"))
                otherWords.Add(ReplaceOccurrence(originalWord, "q", "g"));
            if (originalWord.Contains("g"))
                otherWords.Add(ReplaceOccurrence(originalWord, "g", "q"));

            if (originalWord.Contains("v"))
                otherWords.Add(ReplaceOccurrence(originalWord, "v", "w"));
            if (originalWord.Contains("w"))
                otherWords.Add(ReplaceOccurrence(originalWord, "w", "v"));

            if (originalWord.Contains("l"))
                otherWords.Add(ReplaceOccurrence(originalWord, "l", "r"));
            if (originalWord.Contains("r"))
                otherWords.Add(ReplaceOccurrence(originalWord, "r", "l"));


            return otherWords;
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
            };

            return otherWords;
        }
    }
}
