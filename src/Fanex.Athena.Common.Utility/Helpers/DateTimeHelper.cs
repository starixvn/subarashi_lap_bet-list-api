namespace Fanex.Athena.Common.Utility.Helpers
{
    using System;

    public static class DateTimeHelper
    {
        public static Func<DateTimeOffset> DateTimeOffsetProvider { get; set; } = () => DateTimeOffset.Now;

        public static DateTime Now => DateTimeOffsetProvider().DateTime;

        public static DateTime UtcNow => DateTimeOffsetProvider().UtcDateTime;

        public static DateTime Today => Now.Date;

        public static DateTime MinValue => DateTimeOffset.MinValue.DateTime;

        public static DateTime CreateDateTime(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
            => new DateTimeOffset(year, month, day, hour, minute, second, TimeSpan.Zero).DateTime;

        public static DateTime CreateDateTime() => MinValue;

        public static DateTime GetMaxDate(DateTime dateOne, DateTime dateTwo)
        {
            return dateOne > dateTwo ? dateOne : dateTwo;
        }

        public static DateTime GetMinDate(DateTime dateOne, DateTime dateTwo)
        {
            return dateOne < dateTwo ? dateOne : dateTwo;
        }

        public static bool IsBetween(DateTime date, DateTime minDate, DateTime maxDate)
        {
            return date.Date >= minDate.Date && date.Date <= maxDate.Date;
        }

        public static DateTime GetFirstDateOfWeek(DateTime date, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            const int DaysInAWeek = 7;

            var diff = date.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += DaysInAWeek;
            }

            return date.AddDays(-1 * diff).Date;
        }

        public static bool IsValidMonthRange(DateTime fromDate, DateTime toDate, int maximumMonthRange)
        {
            return toDate >= fromDate && GetMonthDifference(fromDate, toDate) <= maximumMonthRange;
        }

        private static int GetMonthDifference(DateTime fromDate, DateTime toDate)
        {
            const int MonthsInAYear = 12;

            return MonthsInAYear * (toDate.Year - fromDate.Year) + toDate.Month - fromDate.Month + 1;
        }

        public static DateTime ToBeginOfTheDay(DateTime datetime)
        {
            return datetime.Date;
        }

        public static DateTime ToEndOfTheDay(DateTime datetime)
        {
            return datetime.Date.AddDays(1).AddMilliseconds(-3);
        }

        public static DateTime GetFirstDateOfMonth(DateTime datetime)
        {
            return CreateDateTime(datetime.Year, datetime.Month, 1);
        }

        public static DateTime DayOfPreviousWeek(this DateTime datetime, DayOfWeek dayOfWeek)
        {
            int diff = (7 + (datetime.DayOfWeek - dayOfWeek)) % 7;
            return datetime.AddDays(-1 * diff).Date;
        }

        public static DateTime DayOfNextWeek(this DateTime datetime, DayOfWeek dayOfWeek)
        {
            int diff = (7 + (dayOfWeek - datetime.DayOfWeek)) % 7;
            return datetime.AddDays(diff).Date;
        }
    }
}