using System;
using System.Globalization;

namespace ClearTools.extensions
{
    public static partial class DateTimeExtensions
    {
        public static string ToDateString(this DateTime value)
        => value.ToString("dd/MMM/yyy", CultureInfo.InvariantCulture);

        public static string ToDateTimeString(this DateTime value)
        => value.ToString("dd/MMM/yyy HH:mm:ss", CultureInfo.InvariantCulture);
    }
}