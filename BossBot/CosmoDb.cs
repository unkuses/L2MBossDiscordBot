using BossBot.Options;
using CommonLib;
using CommonLib.DBModels;
using CommonLib.Helpers;
using CommonLib.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace BossBot;

public class CosmoDb
{
    private const string DatabaseId = "BossDB";
    private const string ContainerId = "BossTables";
    private readonly CosmosClient _cosmosClient;
    private readonly DateTimeHelper _dateTimeHelper;
    private readonly Dictionary<string, DateTime> _mentionedBosses = [];

    public CosmoDb(DateTimeHelper dateTimeHelper, CosmoDbOptions options)
    {
        _dateTimeHelper = dateTimeHelper;
        var builder = new CosmosClientBuilder(options.Endpoint, options.AccountKey);
        _cosmosClient = builder.Build();

        _ = PopulateTableAsync();
    }

    // Public Methods (Alphabetically Sorted)
    public async Task ClearAllBossInformationAsync(ulong chatId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var query = new QueryDefinition("SELECT c.id, c.BossInformationId FROM c WHERE c.ChatId = @chatId")
            .WithParameter("@chatId", chatId);

        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var item in response)
            {
                await container.DeleteItemAsync<BossInformationDbModel>(item.id, new PartitionKey(item.BossInformationId));
            }
        }
    }

    public async Task<IList<BossModel>> GetAllLoggedBossInfoAsync(ulong chatId)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var query = new QueryDefinition("SELECT * FROM c WHERE c.ChatId = @chatId ORDER BY c.NextRespawnTime ASC")
            .WithParameter("@chatId", chatId);

        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);
        var bossModels = new List<BossModel>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var bossInfo in response)
            {
                bossModels.Add(new BossModel(bossInfo));
            }
        }

        return bossModels
            .OrderBy(b => b.KillTime.AddHours(b.RespawnTime))
            .ToList();
    }

    public async Task<IList<BossModel>> GetFirstLoggedBossInfoAsync(ulong chatId, int count)
    {
        var bossModels = await GetChainBosses(chatId, count);

        return bossModels
            .OrderBy(b => b.KillTime.AddHours(b.RespawnTime))
            .ToList();
    }

    public async Task<IList<BossModel>> GetAllNotLoggedBossInformationAsync(ulong chatId)
    {
        var bossContainer = _cosmosClient.GetContainer(DatabaseId, "BossTables");
        var bossInfoContainer = _cosmosClient.GetContainer(DatabaseId, "BossInformation");

        // Step 1: Retrieve all logged boss IDs for the given chatId
        var loggedBossIdsQuery = new QueryDefinition("SELECT c.BossInformationId FROM c WHERE c.ChatId = @chatId")
            .WithParameter("@chatId", chatId);

        var loggedBossIdsIterator = bossInfoContainer.GetItemQueryIterator<dynamic>(loggedBossIdsQuery);
        var loggedBossIds = new HashSet<string>();

        while (loggedBossIdsIterator.HasMoreResults)
        {
            var response = await loggedBossIdsIterator.ReadNextAsync();
            foreach (var item in response)
            {
                loggedBossIds.Add(item.BossInformationId.ToString());
            }
        }

        // Step 2: Retrieve all bosses that are not logged
        var allBossesQuery = new QueryDefinition("SELECT * FROM c");
        var allBossesIterator = bossContainer.GetItemQueryIterator<BossDbModel>(allBossesQuery);
        var notLoggedBosses = new List<BossModel>();

        while (allBossesIterator.HasMoreResults)
        {
            var response = await allBossesIterator.ReadNextAsync();
            foreach (var boss in response)
            {
                if (!loggedBossIds.Contains(boss.Id))
                {
                    notLoggedBosses.Add(new BossModel(boss));
                }
            }
        }

        return notLoggedBosses;
    }

    private bool IsBossPostpone(BossInformationDbModel info)
    {
        var nextRespawnTime = info.KillTime.AddHours(info.Boss.RespawnTime).AddMinutes(5);
        return _dateTimeHelper.CurrentTime > nextRespawnTime;
    }

    public async Task<IList<BossModel>> GetAndUpdateAllPostponeBossesAsync()
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");

        // Step 1: Query all bosses that need to be postponed
        var query = new QueryDefinition("SELECT * FROM c WHERE c.Boss.RespawnTime > 0");
        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);

        var postponedBosses = new List<BossInformationDbModel>();

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            postponedBosses.AddRange(response.Where(IsBossPostpone));
        }

        if (postponedBosses.Count == 0)
            return new List<BossModel>();

        // Step 2: Update the KillTime for each postponed boss
        foreach (var bossInfo in postponedBosses)
        {
            bossInfo.KillTime = bossInfo.KillTime.AddHours(bossInfo.Boss.RespawnTime);
            bossInfo.NextRespawnTime = bossInfo.KillTime.AddHours(bossInfo.Boss.RespawnTime);

            // Update the item in Cosmos DB
            await container.ReplaceItemAsync(bossInfo, bossInfo.id, new PartitionKey(bossInfo.BossInformationId));
        }

        return postponedBosses.Select(info => new BossModel(info)).ToList();
    }

    public async Task<IList<BossModel>> GetAllAppendingBossesAsync()
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);

        var list = new List<BossModel>();
        var bossInfoList = new List<BossInformationDbModel>();

        // Retrieve all boss information from Cosmos DB
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            bossInfoList.AddRange(response);
        }

        CleanMentionedBossesList();
        // Process each boss information
        foreach (var info in bossInfoList)
        {
            // Only include bosses whose NextRespawnTime is in the next 5 minutes (>= now, < now + 5 min)
            if (info.NextRespawnTime >= _dateTimeHelper.CurrentTime &&
                info.NextRespawnTime < _dateTimeHelper.CurrentTime.AddMinutes(5) &&
                !BossWasMentioned(info))
            {
                list.Add(new BossModel(info));
            }
        }

        return list;
    }

    public async Task<IList<BossModel>> GetAllNotAnnouncedBossesAsync()
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var query = new QueryDefinition("SELECT * FROM c WHERE c.WasMentioned = false");
        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);

        var bossInfoList = new List<BossInformationDbModel>();

        // Retrieve all boss information from Cosmos DB
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            bossInfoList.AddRange(response);
        }

        // Update WasMentioned to true for each retrieved boss
        foreach (var bossInfo in bossInfoList)
        {
            bossInfo.WasMentioned = true;
            await container.ReplaceItemAsync(bossInfo, bossInfo.id, new PartitionKey(bossInfo.BossInformationId));
        }

        return bossInfoList.Select(info => new BossModel(info)).ToList();
    }

    public async Task<IList<BossModel>> GetBossesInformationAsync()
    {
        var bossCache = await GetBossModelsCacheAsync();
        return bossCache.Values
            .Select(b => new BossModel(b))
            .OrderBy(b => b.KillTime)
            .ToList();
    }

    public async Task<BossModel> LogKillBossInformationAsync(ulong chatId, string bossId, DateTime time)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var boss = await GetBossModelByIdAsync(bossId);
        if (boss == null) return null;

        var query = new QueryDefinition("SELECT * FROM c WHERE c.ChatId = @chatId AND c.BossInformationId = @bossId")
            .WithParameter("@chatId", chatId)
            .WithParameter("@bossId", bossId);
        var iterator = container.GetItemQueryIterator<BossInformationDbModel>(query);

        BossInformationDbModel bossInfo = null;
        if (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            bossInfo = response.FirstOrDefault();
        }

        if (bossInfo == null)
        {
            bossInfo = new BossInformationDbModel
            {
                Boss = boss,
                BossInformationId = bossId,
                ChatId = chatId,
                KillTime = time,
                NextRespawnTime = time.AddHours(boss.RespawnTime),
            };
            await container.CreateItemAsync(bossInfo, new PartitionKey(bossInfo.BossInformationId));
        }
        else
        {
            bossInfo.KillTime = time;
            bossInfo.NextRespawnTime = time.AddHours(boss.RespawnTime);
            await container.ReplaceItemAsync(bossInfo, bossInfo.id, new PartitionKey(bossInfo.BossInformationId));
        }

        return new BossModel(bossInfo);
    }

    public async Task PredictedTimeAfterRestartAsync(ulong chatId, DateTime? restartTime)
    {
        if (restartTime == null)
            throw new ArgumentNullException(nameof(restartTime), "Restart time cannot be null.");

        var bossInfoContainer = _cosmosClient.GetContainer(DatabaseId, "BossInformation");
        var bossContainer = _cosmosClient.GetContainer(DatabaseId, "BossTables");

        // Step 1: Retrieve all logged boss IDs for the given chatId
        var loggedBossIdsQuery = new QueryDefinition("SELECT c.BossInformationId FROM c WHERE c.ChatId = @chatId")
            .WithParameter("@chatId", chatId);

        var loggedBossIdsIterator = bossInfoContainer.GetItemQueryIterator<dynamic>(loggedBossIdsQuery);
        var loggedBossIds = new List<string>();

        while (loggedBossIdsIterator.HasMoreResults)
        {
            var response = await loggedBossIdsIterator.ReadNextAsync();
            foreach (var item in response)
            {
                loggedBossIds.Add(item.BossInformationId.ToString());
            }
        }

        // Step 2: Retrieve all bosses
        var allBossesQuery = new QueryDefinition("SELECT * FROM c");
        var allBossesIterator = bossContainer.GetItemQueryIterator<BossDbModel>(allBossesQuery);
        var notLoggedBosses = new List<BossDbModel>();

        while (allBossesIterator.HasMoreResults)
        {
            var response = await allBossesIterator.ReadNextAsync();
            foreach (var boss in response)
            {
                if (!loggedBossIds.Contains(boss.Id))
                {
                    notLoggedBosses.Add(boss);
                }
            }
        }

        // Step 3: Add predicted kill times for not logged bosses
        foreach (var boss in notLoggedBosses)
        {
            var predictedKillTime = boss.RestartRespawnTime == 0
                ? restartTime.Value
                : restartTime.Value.AddHours(boss.RestartRespawnTime - boss.RespawnTime);

            var notLoggedBossInfo = new BossInformationDbModel
            {
                Boss = boss,
                BossInformationId = boss.Id,
                ChatId = chatId,
                KillTime = predictedKillTime,
                NextRespawnTime = predictedKillTime.AddHours(boss.RespawnTime),
            };

            await bossInfoContainer.CreateItemAsync(notLoggedBossInfo, new PartitionKey(notLoggedBossInfo.BossInformationId));
        }
    }

    private async Task<BossDbModel> GetBossModelByIdAsync(string id)
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        try
        {
            // Use the id as the partition key for the search
            var response = await container.ReadItemAsync<BossDbModel>(id, new PartitionKey($"boss{id}"));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            // Return null if the item is not found
            return null;
        }
    }

    private async Task<Dictionary<string, BossDbModel>> GetBossModelsCacheAsync()
    {
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);
        var query = new QueryDefinition("SELECT * FROM c");
        var iterator = container.GetItemQueryIterator<BossDbModel>(query);

        var bossCache = new Dictionary<string, BossDbModel>();
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            foreach (var boss in response)
            {
                bossCache[boss.Name] = boss;
            }
        }

        return bossCache;
    }

    private async Task PopulateTableAsync()
    {
        var allBosses = BossCollection.GetBossesCollection();
        var container = _cosmosClient.GetContainer(DatabaseId, ContainerId);

        foreach (var boss in allBosses)
        {
            try
            {
                var response = await container.ReadItemAsync<BossDbModel>(boss.Id.ToString(), new PartitionKey(boss.BossId));
                var dbModel = response.Resource;

                if (!dbModel.AreEqual(boss))
                {
                    dbModel.RestartRespawnTime = boss.RestartRespawnTime;
                    dbModel.RespawnTime = boss.RespawnTime;
                    dbModel.NickName = boss.NickName;

                    await container.ReplaceItemAsync(dbModel, dbModel.Id.ToString(), new PartitionKey(dbModel.BossId));
                }
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await container.CreateItemAsync(boss, new PartitionKey(boss.BossId));
            }
        }
    }

    private async Task<List<BossModel>> GetChainBosses(ulong chatId, int count)
    {
        var allBosses = await GetAllLoggedBossInfoAsync(chatId);
        var result = new List<BossModel>();
        var previousRespawnTime = DateTime.MinValue;
        foreach (var bossModel in allBosses.OrderBy(b => b.KillTime.AddHours(b.RespawnTime)))
        {
            if(count < 0 && previousRespawnTime.AddMinutes(5) < bossModel.NextRespawnTime) 
                break;
            result.Add(bossModel);
            previousRespawnTime = bossModel.NextRespawnTime;
            count--;
        }

        return result;
    }

    private void CleanMentionedBossesList()
    {
        var list = _mentionedBosses.Where(kvp => kvp.Value > _dateTimeHelper.CurrentTime).ToList();
        list.ForEach(id => _mentionedBosses.Remove(id.Key));
    }

    private bool BossWasMentioned(BossInformationDbModel boss)
    {
        if(_mentionedBosses.ContainsKey(boss.id))
        {
            return true;
        }
        _mentionedBosses.Add(boss.id, boss.NextRespawnTime);
        return false;
    }
}