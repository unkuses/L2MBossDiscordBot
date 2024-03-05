using BossBot.Interfaces;

namespace BossBot.Commands
{
    public class RestartTimeCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["r", "р"];
        public Task<IEnumerable<string>> ExecuteAsync(ulong chatId, string[] commands)
        {
            var list = new List<string>();
            var time = ParseDateTimeParameters(commands);
            if (!time.HasValue)
            {
                list.Add("Не правильный формат");
            }
            else
            {
                bossData.PredictedTimeAfterRestart(chatId, time.Value);
            }

            return Task.FromResult(list.Select(s => s));
        }

        private DateTime? ParseDateTimeParameters(string[] parameters)
        {
            switch (parameters.Length)
            {
                case 2:
                    if (!DateTime.TryParse(parameters[1], out var dateTime))
                    {
                        return null;
                    }

                    if (dateTime > dateTimeHelper.CurrentTime.AddHours(1))
                        dateTime = dateTime.AddDays(-1);
                    return dateTime;
                case 3:
                    if (!DateTime.TryParse($"{parameters[1]} {parameters[2]}", out dateTime))
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
