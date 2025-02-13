using System;
using System.Globalization;

namespace Clear.Tools
{
    public static class DateUtility
    {
        public static bool IsSameWeek(DateTime? date1, DateTime? date2, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            if (!date1.HasValue || !date2.HasValue)
            {
                return false;
            }

            var cal = CultureInfo.CurrentCulture.Calendar;

            return cal.GetWeekOfYear(date1.Value, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek) ==
                   cal.GetWeekOfYear(date2.Value, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek) &&
                   date1.Value.Year == date2.Value.Year;
        }
    }
}