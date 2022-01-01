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

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyBlazorApp.Server.Helpers
{
    public static class Helper
    {
        public static string ApplyStyleToText(string exampleUse)
        {
            if (!string.IsNullOrEmpty(exampleUse) && exampleUse.Contains("*"))
            {
                var start = exampleUse.IndexOf('*');
                var end = exampleUse.LastIndexOf('*');
                var originalWord = exampleUse.Substring(start + 1, end - start - 1).ToUpper();
                return exampleUse.Substring(0, start)
                    + originalWord
                    + exampleUse.Substring(end + 1);
            }
            return exampleUse;
        }

        public static string HideOriginal(string exampleUse)
        {
            if (!string.IsNullOrEmpty(exampleUse) && exampleUse.Contains("*"))
            {
                var start = exampleUse.IndexOf('*');
                var end = exampleUse.LastIndexOf('*');
                return exampleUse.Substring(0, start)
                    + "_____"
                    + exampleUse.Substring(end + 1);
            }
            return exampleUse;
        }

        public static string GetOriginalFromExample(string exampleUse)
        {
            if (!string.IsNullOrEmpty(exampleUse) && exampleUse.Contains("*"))
            {
                var start = exampleUse.IndexOf('*');
                var end = exampleUse.LastIndexOf('*');
                var originalWord = exampleUse.Substring(start + 1, end - start - 1).ToLower();
                return originalWord;
            }
            return string.Empty;
        }


        public static bool IsCorrectTranslation(string translation, string userAnswer)
        {
            if(translation.ToLower().Trim() == userAnswer.ToLower().Trim())
            {
                return true;
            }
            else
            {
                var splitted = translation.Split(',').Select(s => s.ToLower().Trim()).ToList();
                var allWords = new List<string>();
                foreach (var word in splitted)
                {
                    allWords.Add(word);
                    if(word.Contains("(") && word.Contains(")"))
                    {
                        var withoutTextInBrackets = Regex.Replace(word, @" ?\(.*?\)", string.Empty);
                        var withoutBrackets = word.Replace("(", string.Empty).Replace(")", string.Empty);
                        allWords.Add(withoutTextInBrackets.ToLower().Trim());
                        allWords.Add(withoutBrackets.ToLower().Trim());
                    }
                }
                
                return allWords.Contains(userAnswer.ToLower().Trim());
            }
        }
    }
}
