using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Toggles the string between empty and the specified value.
        /// </summary>
        public static string Toggle(this string str, string value)
        => string.IsNullOrEmpty(str) ? value : string.Empty;

        /// <summary>
        /// Toggles the string between two specified values.
        /// </summary>
        public static string Toggle(this string str, string value1, string value2)
        => str == value1 ? value2 : value1;

        /// <summary>
        /// Searches for a value in the string, ignoring case.
        /// </summary>
        public static bool Search(this string text, string value)
        => text.ToLower().Contains(value.ToLower());

        /// <summary>
        /// Strips symbols from the string.
        /// </summary>
        public static string StripSymbols(this string text)
        => Regex.Replace(text, "[;\\\\/:*?\"<>|&'+`',/\\(\\)\\[\\]{}\\\"#*]", string.Empty);

        /// <summary>
        /// Extracts numbers from the string.
        /// </summary>
        public static string ExtractNumbers(this string value)
        => Regex.Replace(value, "[^0-9.-]", "").Trim();

        /// <summary>
        /// Converts the string to an integer.
        /// </summary>
        public static int ToInt32(this string value)
        => Convert.ToInt32(value.ExtractNumbers());

        /// <summary>
        /// Converts the string to a decimal.
        /// </summary>
        public static decimal ToDecimal(this string value)
        => Convert.ToDecimal(value.ExtractNumbers());

        /// <summary>
        /// Converts the string to a double.
        /// </summary>
        public static double ToDouble(this string value)
        => Convert.ToDouble(value.ExtractNumbers());

        /// <summary>
        /// Converts the string to a DateTime.
        /// </summary>
        public static DateTime ToDateTime(this string value)
        => Convert.ToDateTime(value);

        /// <summary>
        /// Converts the string from a Base64 string to a byte array.
        /// </summary>
        public static byte[] FromBase64String(this string value)
        => Convert.FromBase64String(value);

        /// <summary>
        /// Compares the string to another string, ignoring case.
        /// </summary>
        public static bool EqualsNoCase(this string text, string value)
        => text.ToLower() == value.ToLower();

        /// <summary>
        /// Converts a CSV string to a list of strings.
        /// </summary>
        public static List<string> ToListFromCsv(this string csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? new List<string>()
                : new List<string>(csv.Split(',').Select(item => item.Trim()));
        }

        /// <summary>
        /// Converts a CSV string to a hash set of strings.
        /// </summary>
        public static HashSet<string> ToHashSetFromCsv(this string csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? new HashSet<string>()
                : new HashSet<string>(csv.Split(',').Select(item => item.Trim()));
        }
    }
}