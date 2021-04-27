﻿// Copyright (C) 2021  Kamil Bugno
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
            return string.Empty;
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
    }
}