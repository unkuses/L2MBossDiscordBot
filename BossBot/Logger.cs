namespace BossBot
{
    public class Logger
    {
        private const string FilePath = "Log.log";

        public Task WriteLog(string message) =>
            File.AppendAllTextAsync(FilePath, $"{DateTime.Now}: {Environment.NewLine}{message}");
    }
}
