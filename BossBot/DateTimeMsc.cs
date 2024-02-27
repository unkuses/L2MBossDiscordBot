namespace BossBot
{
    public static class DateTimeMsc
    {
        private static TimeZoneInfo TimeZoneInfoMsc { get; } = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        public static DateTime CurrentTimeMcs => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfoMsc);
    }
}