﻿using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot
{
    public class BossData
    {
        private readonly BossDataSource _bossData = new();
        private List<BossDbModel> _bossCache;
        private readonly List<int> _mentionedBosses = new();

        public BossData()
        {
            var needPopulateTable = _bossData.DatabaseIsCreated;
            _bossData.Database.EnsureCreated();
            if (!needPopulateTable)
            {
                PopulateTable();
            }
        }

        private void PopulateTable()
        {
            _bossData.BossDbModels.AddRange(BossCollection.GetBossesCollection());
            _bossData.SaveChanges();
        }

        private List<BossDbModel> BossModelsCache
        {
            get
            {
                if (_bossCache == null)
                {
                    _bossCache = _bossData.BossDbModels.OrderBy(b => b.ID).ToList();
                }

                return _bossCache;
            }
        }

        private BossDbModel GetBossModelById(int id) => BossModelsCache[id - 1];

        public IList<BossModel> GetBossesInformation() =>
            BossModelsCache.Select(b => new BossModel(b)).OrderBy(b => b.KillTime).ToList();

        private bool IsBossPostpone(BossInformationDbModel info)
        {
            var nextRespawnTime = info.KillTime.AddHours(GetBossModelById(info.BossId).RespawnTime).AddMinutes(5);
            return DateTimeMsc.CurrentTimeMcs > nextRespawnTime;
        }


        public IList<BossModel> GetAllNotLoggedBossInformation(ulong chatId)
            => _bossData.BossDbModels
                .Where(b => _bossData.BossInformationDbModels.All(info => info.ChatId == chatId && info.BossId != b.ID))
                .Select(b => new BossModel(b)).ToList();

        public BossModel LogKillBossInformation(ulong chatId, int bossId, DateTime time)
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
                if (info.KillTime.AddHours(info.Boss.RespawnTime) < DateTimeMsc.CurrentTimeMcs.AddMinutes(5) &&
                    !_mentionedBosses.Contains(info.Id))
                {
                    _mentionedBosses.Add(info.Id);
                    list.Add(new BossModel(info));
                }
            });
            return list;
        }
    }
}