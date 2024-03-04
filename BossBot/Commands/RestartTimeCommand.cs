using BossBot.Interfaces;
using Discord.WebSocket;

namespace BossBot.Commands
{
    public class RestartTimeCommand(BossData bossData, DateTimeHelper dateTimeHelper) : ICommand
    {
        public string[] Keys { get; } = ["r", "р"];
        public Task ExecuteAsync(ISocketMessageChannel channel, string[] commands)
        {
            var time = ParseDateTimeParameters(commands);
            if (!time.HasValue)
            {
                channel.SendMessageAsync("Не правильный формат");
                return Task.CompletedTask;
            }
            return bossData.PredictedTimeAfterRestart(channel.Id, time.Value);
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
