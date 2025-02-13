using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClearTools.extensions
{
    public static class StringExtensions
    {
        public static string Toggle(this string str, string value)
        => string.IsNullOrEmpty(str) ? value : string.Empty;

        public static string Toggle(this string str, string value1, string value2)
        => str == value1 ? value2 : value1;

        public static bool Search(this string text, string value)
        => text.ToLower().Contains(value.ToLower());

        public static string StripSymbols(this string text)
        => Regex.Replace(text, "[;\\\\/:*?\"<>|&'+`',/\\(\\)\\[\\]{}\\\"#*]", string.Empty);

        public static string ExtractNumbers(this string value)
        => Regex.Replace(value, "[^0-9.-]", "").Trim();

        public static int ToInt32(this string value)
        => Convert.ToInt32(value.ExtractNumbers());

        public static decimal ToDecimal(this string value)
        => Convert.ToDecimal(value.ExtractNumbers());

        public static double ToDouble(this string value)
        => Convert.ToDouble(value.ExtractNumbers());

        public static DateTime ToDateTime(this string value)
        => Convert.ToDateTime(value);

        public static byte[] FromBase64String(this string value)
        => Convert.FromBase64String(value);

        public static bool EqualsNoCase(this string text, string value)
        => text.ToLower() == value.ToLower();

        public static List<string> ToListFromCsv(this string csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? new List<string>()
                : new List<string>(csv.Split(',').Select(item => item.Trim()));
        }

        public static HashSet<string> ToHashSetFromCsv(this string csv)
        {
            return string.IsNullOrWhiteSpace(csv)
                ? new HashSet<string>()
                : new HashSet<string>(csv.Split(',').Select(item => item.Trim()));
        }
    }
}