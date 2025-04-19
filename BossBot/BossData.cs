using BossBot.DBModel;

namespace BossBot
{
    public class BossData
    {
        private readonly BossDataSource _bossData = new();

        public BossData(Options options)
        {
            _bossData.Database.EnsureCreated();
        }
        
        public string SetUserTimeZone(ulong userId, string timeZone)
        {
            var existUser = _bossData.UserInformationDbModels.FirstOrDefault(u => u.UserId == userId);
            if (existUser != null)
            {
                existUser.UserTimeZone = timeZone;
            }
            else
            {
                _bossData.UserInformationDbModels.Add(new UserInformationDBModel()
                    { UserId = userId, UserTimeZone = timeZone });
            }

            _bossData.SaveChanges();
            return $"Time zone set as {timeZone}";
        }

        public string GetUserTimeZone(ulong userId)
        {
            if (_bossData.UserInformationDbModels.Any(u => u.UserId == userId))
            {
                return _bossData.UserInformationDbModels.First(u => u.UserId == userId).UserTimeZone;
            }

            return string.Empty;
        }
    }
}