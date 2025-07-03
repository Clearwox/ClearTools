using System;
using System.Globalization;

namespace ClearTools.Extensions
{
    /// <summary>
    /// Extension methods for DateTime.
    /// </summary>
    public static partial class DateTimeExtensions
    {
        /// <summary>
        /// Converts the DateTime to a string in the format "dd/MMM/yyyy".
        /// </summary>
        public static string ToDateString(this DateTime value)
        => value.ToString("dd/MMM/yyyy", CultureInfo.InvariantCulture);

        /// <summary>
        /// Converts the DateTime to a string in the format "dd/MMM/yyyy HH:mm:ss".
        /// </summary>
        public static string ToDateTimeString(this DateTime value)
        => value.ToString("dd/MMM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
}