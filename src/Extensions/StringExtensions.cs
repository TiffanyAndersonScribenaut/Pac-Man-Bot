﻿using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace PacManBot.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Returns the given text if the condition is true, otherwise returns an empty string.
        /// Helps a little with long text concatenation.</summary>
        public static string If(this string text, bool condition) => condition ? text : "";


        /// <summary>Retrieves a string with the end trimmed off if the original exceeds the provided maximum length.</summary>
        public static string Truncate(this string value, int maxLength)
            => value.Substring(0, Math.Min(maxLength, value.Length));


        /// <summary>Retrieves a string with the beginning trimmed off if the original exceeds the provided maximum length.</summary>
        public static string TruncateStart(this string value, int maxLength)
            => value.Substring(Math.Max(0, value.Length - maxLength));


        /// <summary>Determines whether the beginning or end of a string matches the specified value.</summary>
        public static bool StartsOrEndsWith(this string text, string value) => text.StartsWith(value) || text.EndsWith(value);

        /// <summary>Determines whether the beginning or end of a string matches the specified value
        /// when compared using the specified comparison option.</summary>
        public static bool StartsOrEndsWith(this string text, string value, StringComparison comparisonType)
            => text.StartsWith(value, comparisonType) || text.EndsWith(value, comparisonType);


        /// <summary>Whether any of the provided values occur within a string.</summary>
        public static bool ContainsAny(this string text, params string[] values) => values.Any(text.Contains);

        /// <summary>Whether any of the values in a collection occur within a string.</summary>
        public static bool ContainsAny(this string text, IEnumerable<string> values) => values.Any(text.Contains);

        /// <summary>Whether any of the provided characters occur within a string.</summary>
        public static bool ContainsAny(this string text, params char[] values) => values.Any(text.Contains);

        /// <summary>Whether any of the characters in a collection occur within a string.</summary>
        public static bool ContainsAny(this string text, IEnumerable<char> values) => values.Any(text.Contains);


        /// <summary>Replaces all occurrences of the specified "before" values
        /// with their corresponding "after" values, inside a string.</summary>
        public static string ReplaceMany(this string text, params (string before, string after)[] replacements)
            => text.ReplaceMany((IEnumerable<(string, string)>)replacements);

        /// <summary>Replaces all occurrences of the specified "before" values
        /// with their corresponding "after" values, inside a string.</summary>
        public static string ReplaceMany(this string text, IEnumerable<(string before, string after)> replacements)
        {
            var sb = new StringBuilder(text);
            foreach (var (before, after) in replacements) sb.Replace(before, after);
            return sb.ToString();
        }


        /// <summary>
        /// Returns the percentage similarity of two strings using a Levenshtein algorithm.
        /// </summary>
        // https://social.technet.microsoft.com/wiki/contents/articles/26805.c-calculating-percentage-similarity-of-2-strings.aspx
        public static double Similarity(this string a, string b, bool caseSensitive = true)
        {
            if (a == null || b == null) return 0.0;

            if (!caseSensitive)
            {
                a = a.ToLowerInvariant();
                b = b.ToLowerInvariant();
            }

            if (a == b) return 1.0;
            if (a.Length == 0 || b.Length == 0) return 0.0;

            int[,] distance = new int[a.Length + 1, b.Length + 1];

            for (int i = 0; i <= a.Length; distance[i, 0] = i++) ;
            for (int j = 0; j <= b.Length; distance[0, j] = j++) ;

            for (int i = 1; i <= a.Length; i++)
            {
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = (b[j - 1] == a[i - 1]) ? 0 : 1;
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            int steps = distance[a.Length, b.Length];
            return 1.0 - (double)steps / Math.Max(a.Length, b.Length);
        }


        /// <summary>
        /// Returns a Python-like string slice that is between the specified boundaries and takes characters by the specified step.
        /// </summary>
        /// <param name="start">The starting index of the slice. Loops around if negative.</param>
        /// <param name="stop">The index the slice goes up to, excluding itself. Loops around if negative.</param>
        /// <param name="step">The increment between each character index. Traverses backwards if negative.</param>
        public static string Slice(this string str, int? start = null, int? stop = null, int step = 1)
        {
            return CollectionExtensions.Slice(str, start, stop, step).JoinString();
        }


        /// <summary>Returns a string with the original value repeated the specified amount of times.</summary>
        public static string Repeat(this string value, int amount)
        {
            if (amount == 1) return value;

            var sb = new StringBuilder(amount * value.Length);
            for (int i = 0; i < amount; i++) sb.Append(value);
            return sb.ToString();
        }
    }
}