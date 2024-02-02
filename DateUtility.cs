using System;
using System.Globalization;

namespace Clear
{
    public class DateUtility
    {
        public static bool IsSameWeek(DateTime? date1, DateTime? date2, DayOfWeek firstDayOfWeek = DayOfWeek.Monday)
        {
            if (date1.HasValue && date2.HasValue)
            {
                GregorianCalendar cal = new GregorianCalendar();
                return cal.GetWeekOfYear(date1.Value, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek) ==
                       cal.GetWeekOfYear(date2.Value, CalendarWeekRule.FirstFourDayWeek, firstDayOfWeek);
            }
            return false;
        }
    }
}