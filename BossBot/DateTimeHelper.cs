namespace BossBot
{
    public class DateTimeHelper(string timeZone)
    {
        private TimeZoneInfo TimeZoneInfoMsc { get; } = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        public DateTime CurrentTime => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfoMsc);
    }
}