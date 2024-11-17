using System.Globalization;

namespace BossBot
{
    public class DateTimeHelper(string timeZone)
    {
        private TimeZoneInfo TimeZoneInfo { get; } = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        public DateTime CurrentTime => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo);

        public DateTime? ParseCommand(params string[] command)
        {
            switch (command.Length)
            {
                case 1:
                    if (!DateTime.TryParse(command[0], out var dateTime))
                    {
                        return null;
                    }

                    var currentTime = CurrentTime;
                    dateTime = new DateTime(new DateOnly(currentTime.Year, currentTime.Month, currentTime.Day),
                        new TimeOnly(dateTime.Hour, dateTime.Minute));
                    return dateTime;
                case 2:
                    if (!DateTime.TryParse($"{command[0]} {command[1]}", out dateTime))
                    {
                        return null;
                    }

                    return dateTime;
                default:
                    return null;
            }
        }

        public DateTime NormalizeDateTime(DateTime input, string timeZone)
        {
            var fromTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(input, fromTimeZone);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, TimeZoneInfo);
        }

        public DateTime? TryParseData(string input, string timeZoneInfo)
        {
            string format = "dd.MM.yyyy HH:mm";
            string formatUS = "yyyy/MM/dd HH:mm";
            IFormatProvider provider = new CultureInfo("en-US");
            if (DateTime.TryParseExact(input, format, provider, DateTimeStyles.None, out var data))
            {
                if (!string.IsNullOrEmpty(timeZoneInfo))
                {
                    data = NormalizeDateTime(data, timeZoneInfo);
                }

                return data;
            }
            if (DateTime.TryParseExact(input, formatUS, provider, DateTimeStyles.None, out var dataUS))
            {
                if (!string.IsNullOrEmpty(timeZoneInfo))
                {
                    dataUS = NormalizeDateTime(dataUS, timeZoneInfo);
                }

                return dataUS;
            }

            return null;
        }
    }
}