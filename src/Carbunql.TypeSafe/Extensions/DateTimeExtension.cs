namespace Carbunql.TypeSafe.Extensions;

public static class DateTimeExtension
{
    public static DateTime TruncateToYear(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, 1, 1);
    }

    public static DateTime TruncateToQuarter(this DateTime dateTime)
    {
        int quarter = (dateTime.Month - 1) / 3 + 1;
        return new DateTime(dateTime.Year, quarter * 3 - 2, 1);
    }

    public static DateTime TruncateToMonth(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1);
    }

    public static DateTime TruncateToDay(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
    }

    public static DateTime TruncateToHour(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
    }

    public static DateTime TruncateToMinute(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
    }

    public static DateTime TruncateToSecond(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
    }

    public static DateTime ToMonthEndDate(this DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
    }
}
