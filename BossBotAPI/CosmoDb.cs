using CommonLib;
using CommonLib.DBModels;
using CommonLib.Helpers;
using CommonLib.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace BossBotAPI
{
    public class CosmoDb
    {
        private const string DatabaseId = "BossDB";
        private const string ContainerId = "BossTables";
        private readonly CosmosClient _cosmosClient;
        private readonly DateTimeHelper _dateTimeHelper;

        public CosmoDb(DateTimeHelper dateTimeHelper, string endpoint, string accountKey)
        {
            _dateTimeHelper = dateTimeHelper;
            var builder = new CosmosClientBuilder(endpoint, accountKey);
            _cosmosClient = builder.Build();
        }

        public async Task<List<BossModel>> ImageAnalyzeParser(List<string> lines, ulong chatId, string timeZoneInfo, bool wasMentioned)
        {
            var bossInfo = new List<BossModel>();
            var dateTimes = new List<DateTime>();

            for (int i = 0; i < lines.Count; i++)
            {
                var bossModelsCache = BossCollection.GetBossesCollection();
                // Find the boss in the cache whose name matches the current line
                var boss = bossModelsCache
                    .FirstOrDefault(b => b.BossNames
                        .Any(name => lines[i].Contains(name.Name, StringComparison.CurrentCultureIgnoreCase)));

                if (boss != null)
                {
                    i++; // Move to the next line to start parsing dates
                    for (; i < lines.Count; i++)
                    {
                        // Try to parse a date from the current line
                        var parsedDate = _dateTimeHelper.TryParseData(lines[i], timeZoneInfo);
                        if (parsedDate.HasValue)
                        {
                            dateTimes.Add(parsedDate.Value);
                        }

                        // Check if the next line contains another boss name or if it's the last line
                        var isNextBoss = bossModelsCache
                            .Any(b => b.BossNames
                                .Any(name => lines[i].Contains(name.Name, StringComparison.CurrentCultureIgnoreCase)));

                        if ((isNextBoss || i == lines.Count - 1) && dateTimes.Any())
                        {
                            // Log the kill information and add it to the result
                            var bossModel = await LogKillBossInformationAsync(chatId, boss.Id, dateTimes.Min(), wasMentioned);
                            dateTimes.Clear();
                            if (bossModel != null)
                            {
                                bossInfo.Add(bossModel);
                            }

                            i--; // Step back to reprocess the current line in the outer loop
                            break;
                        }
                    }
                }
                else
                {
                    // Try to parse a date if no boss is found
                    var parsedDate = _dateTimeHelper.TryParseData(lines[i], timeZoneInfo);
                    if (parsedDate.HasValue)
                    {
                        dateTimes.Add(parsedDate.Value);
                    }
                }
            }

            return bossInfo;
        }

        public async Task<BossModel?> LogKillBossInformationAsync(ulong chatId, string bossId, DateTime time, bool wasMentioned)
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
                    WasMentioned = wasMentioned
                };
                await container.CreateItemAsync(bossInfo, new PartitionKey(bossInfo.BossInformationId));
            }
            else
            {
                if (bossInfo.KillTime != time)
                {
                    bossInfo.KillTime = time;
                    bossInfo.NextRespawnTime = time.AddHours(boss.RespawnTime);
                    bossInfo.WasMentioned = wasMentioned;
                    await container.ReplaceItemAsync(bossInfo, bossInfo.id,
                        new PartitionKey(bossInfo.BossInformationId));
                }
            }

            return new BossModel(bossInfo);
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
    }
}
