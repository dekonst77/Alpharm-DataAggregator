using System;

namespace DataAggregator.Web.Managers
{
    public static class DateTimeExtensions
    {
        public static int MonthDistance(this DateTime startDateTime, DateTime endDateTime)
        {
            return (endDateTime.Year - startDateTime.Year) * 12 + endDateTime.Month - startDateTime.Month;
        }
    }
}