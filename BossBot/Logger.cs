namespace BossBot
{
    public class Logger
    {
        private const string FilePath = "Log.log";

        public Task WriteLog(string message) =>
            File.WriteAllTextAsync(FilePath, $"{Environment.NewLine}{message}");
    }
}
