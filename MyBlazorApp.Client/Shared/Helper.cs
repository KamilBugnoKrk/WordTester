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

using MyBlazorApp.Shared;

namespace MyBlazorApp.Client.Shared
{
    public static class Helper
    {
        public static string ApplyStyleToText(string example)
        {
            if (example.Contains("*"))
            {
                var start = example.IndexOf('*');
                var end = example.LastIndexOf('*');
                var originalWord = example.Substring(start + 1, end - start - 1).ToUpper();
                return example.Substring(0, start) 
                    + originalWord 
                    + example.Substring(end + 1);
            }
            return example;
        }
    }
}
