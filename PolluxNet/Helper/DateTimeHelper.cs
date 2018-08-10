using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Helper
{
    public static class DateTimeHelper
    {
        public static int GetWeekOfYear(DateTime value)
        {
            System.Globalization.GregorianCalendar getWeek = new System.Globalization.GregorianCalendar();
            return getWeek.GetWeekOfYear(value, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Sunday);//設定星期日為一週的第一天
        }
        public static string FromWeekOfYear(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)CultureInfo.InvariantCulture.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);

            return firstWeekDay.AddDays(weekOfYear * 7).ToString("yyyy/MM/dd");
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if (firstWeek <= 1 || firstWeek > 50)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
        public static double ToTimestamp(this DateTime date)
        {
            //create Timespan by subtracting the date provided from the Unix Epoch
            TimeSpan span = (date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0));
            //return the total seconds (which is a UNIX timestamp)
            return (double)span.TotalSeconds;
        }
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }
        /// <summary>
        /// Convert Unix Timestamp to LocalTime
        /// </summary>
        /// <param name="unixTimeStamp">Unix Timestamp</param>
        /// <returns>本地時間</returns>
        /// 
        public static DateTime FromTimestamp(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }
}
