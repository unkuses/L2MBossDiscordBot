namespace BossBot
{
    public class DateTimeHelper(string timeZone)
    {
        private TimeZoneInfo TimeZoneInfoMsc { get; } = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        public DateTime CurrentTime => TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfoMsc);

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
    }
}