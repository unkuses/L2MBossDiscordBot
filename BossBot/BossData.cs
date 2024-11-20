using System.Globalization;
using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot
{
    public class BossData
    {
        private readonly BossDataSource _bossData = new();
        private List<BossDbModel>? _bossCache;
        private readonly List<int> _mentionedBosses = new();
        private readonly DateTimeHelper _dateTimeHelper;

        public BossData(Options options)
        {
            _dateTimeHelper = new DateTimeHelper(options.TimeZone);
            _bossData.Database.EnsureCreated();
            PopulateTable();
        }

        private void PopulateTable()
        {
            var allBosses = BossCollection.GetBossesCollection();
            foreach (var boss in allBosses)
            {
                var dbModel = _bossData.BossDbModels.FirstOrDefault(b => Equals(b.Name, boss.Name));
                if (dbModel == null)
                {
                    _bossData.BossDbModels.Add(boss);
                }
                else if (!dbModel.AreEqual(boss))
                {
                    dbModel.RestartRespawnTime = boss.RestartRespawnTime;
                    dbModel.RespawnTime = boss.RespawnTime;
                    dbModel.NickName = boss.NickName;
                }
            }

            _bossData.SaveChanges();
        }

        private List<BossDbModel> BossModelsCache
        {
            get
            {
                if (_bossCache == null)
                {
                    _bossCache = _bossData.BossDbModels.Include(b => b.BossNames).OrderBy(b => b.ID).ToList();
                }

                return _bossCache;
            }
        }

        private BossDbModel GetBossModelById(int id) => BossModelsCache[id - 1];

        public IList<BossModel> GetBossesInformation() =>
            BossModelsCache.Select(b => new BossModel(b)).OrderBy(b => b.KillTime).ToList();
        
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

        public List<BossModel> ImageAnalyzeParser(List<string> lines, ulong userId, ulong chatId)
        {
            List<BossModel> bossInfo = new List<BossModel>();
            UserInformationDBModel timeZoneInfo = null;
            if (_bossData.UserInformationDbModels.Any(u => u.UserId == userId))
            {
                timeZoneInfo = _bossData.UserInformationDbModels.First(u => u.UserId == userId);
            }
            List<DateTime> dateTimes = new List<DateTime>();
            for (int i = 0; i < lines.Count; i++)
            {
                var boss = BossModelsCache.FirstOrDefault(b => b.BossNames.Any(name => lines[i].Contains(name.Name, StringComparison.CurrentCultureIgnoreCase)));
                if (boss != null)
                {
                    i = i + 1;
                    for (;i<lines.Count;i++)
                    {
                        var data = _dateTimeHelper.TryParseData(lines[i], timeZoneInfo?.UserTimeZone);
                        if (data.HasValue)
                        {
                            dateTimes.Add(data.Value);
                        }
                        if (BossModelsCache.Any(b => b.BossNames.Any(name => lines[i].Contains(name.Name, StringComparison.CurrentCultureIgnoreCase))) || i == lines.Count - 1)
                        {
                            var bossModel = LogKillBossInformation(chatId, boss.ID, dateTimes.Min());
                            dateTimes.Clear();
                            bossInfo.Add(bossModel);
                            --i;
                            break;
                        }
                    }
                }
                else
                {
                    var data = _dateTimeHelper.TryParseData(lines[i], timeZoneInfo?.UserTimeZone);
                    if (data.HasValue)
                    {
                        dateTimes.Add(data.Value);
                    } 
                }
            }

            return bossInfo;
        }

        private bool IsBossPostpone(BossInformationDbModel info)
        {
            var nextRespawnTime = info.KillTime.AddHours(GetBossModelById(info.BossId).RespawnTime).AddMinutes(5);
            return _dateTimeHelper.CurrentTime > nextRespawnTime;
        }


        public IList<BossModel> GetAllNotLoggedBossInformation(ulong chatId)
            => _bossData.BossDbModels
                .Where(b => _bossData.BossInformationDbModels.Any(info => info.ChatId == chatId && info.BossId != b.ID))
                .Select(b => new BossModel(b)).ToList();

        public BossModel? LogKillBossInformation(ulong chatId, int bossId, DateTime time)
        {
            var boss = GetBossModelById(bossId);
            if (boss == null) return null;
            var bossInfo =
                _bossData.BossInformationDbModels.FirstOrDefault(info =>
                    info.ChatId == chatId && info.BossId == bossId);
            if (bossInfo == null)
            {
                bossInfo = new BossInformationDbModel
                    { Boss = boss, BossId = bossId, ChatId = chatId, KillTime = time };
                _bossData.BossInformationDbModels.Add(bossInfo);
            }
            else
            {
                bossInfo.KillTime = time;
            }

            _bossData.SaveChangesAsync();
            _mentionedBosses.Remove(bossInfo.Id);
            return new BossModel(bossInfo);
        }

        public IList<BossModel> GetAllLoggedBossInfo(ulong chatId)
        {
            var bosses = _bossData.BossInformationDbModels.Where(info => info.ChatId == chatId).ToList();
            return bosses.Select(b => new BossModel(b)).OrderBy(b => b.KillTime.AddHours(b.RespawnTime)).ToList();
        }

        public IList<BossModel> GetFirstLoggedBossInfo(ulong chatId, int count)
        {
            var bosses = _bossData.BossInformationDbModels
                .Include(bossInformationDbModel => bossInformationDbModel.Boss)
                .Where(info => info.ChatId == chatId)
                .OrderBy(i => i.KillTime.AddHours(i.Boss.RespawnTime))
                .Take(count)
                .ToList();
            return bosses.Select(b => new BossModel(b)).ToList();
        }

        public IList<BossModel> GetAndUpdateAllPostponeBosses()
        {
            var bossInfo = _bossData.BossInformationDbModels
                .Include(bossInformationDbModel => bossInformationDbModel.Boss).ToList().Where(IsBossPostpone).ToList();
            if (bossInfo.Count == 0)
                return new List<BossModel>();
            bossInfo.ForEach(info =>
            {
                info.KillTime = info.KillTime.AddHours(info.Boss.RespawnTime);
                _mentionedBosses.Remove(info.Id);
            });
            _bossData.SaveChangesAsync();
            return bossInfo.Select(info => new BossModel(info)).ToList();
        }

        public IList<BossModel> GetAllAppendingBosses()
        {
            var list = new List<BossModel>();
            var bossInfo = _bossData.BossInformationDbModels
                .Include(bossInformationDbModel => bossInformationDbModel.Boss).ToList();
            bossInfo.ForEach(info =>
            {
                if (info.KillTime.AddHours(info.Boss.RespawnTime) < _dateTimeHelper.CurrentTime.AddMinutes(5) &&
                    !_mentionedBosses.Contains(info.Id))
                {
                    _mentionedBosses.Add(info.Id);
                    list.Add(new BossModel(info));
                }
            });
            return list;
        }

        public IList<BossModel> GetAllAdenByLocation(ulong chatId, string location) =>
            _bossData.BossInformationDbModels.Include(model => model.Boss)
                .Where(info => info.ChatId == chatId && info.Boss.Location == location).Select(i => new BossModel(i))
                .ToList();


        public void ClearAllBossInformation(ulong chatId)
        {
            _bossData.BossInformationDbModels.RemoveRange(
                _bossData.BossInformationDbModels.Where(info => info.ChatId == chatId));
            _bossData.SaveChangesAsync();
        }

        public Task PredictedTimeAfterRestart(ulong chatId, DateTime? restartTime)
        {
            var bossInfo = _bossData.BossInformationDbModels.Where(info => info.ChatId == chatId);
            var notLoggedBosses = BossModelsCache.Where(b => !bossInfo.Any(info => info.BossId == b.ID));

            foreach (var boss in notLoggedBosses)
            {
                var predictedKillTime = boss.RestartRespawnTime == 0
                    ? restartTime.Value
                    : restartTime.Value.AddHours(boss.RestartRespawnTime - boss.RespawnTime);
                var notLoggedBossInfo = new BossInformationDbModel()
                {
                    Boss = boss,
                    BossId = boss.ID,
                    ChatId = chatId,
                    KillTime = predictedKillTime
                };
                _bossData.BossInformationDbModels.Add(notLoggedBossInfo);
            }

            return _bossData.SaveChangesAsync();
        }
    }
}