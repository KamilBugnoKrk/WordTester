using System;
using System.Collections.Generic;

namespace MyBlazorApp.Server.LearningAlgorithm
{
    public static class Extensions
    {
        private static readonly Random rng = new();

        // Based on https://stackoverflow.com/questions/273313/randomize-a-listt
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
