using BossBot.DataSource;
using Microsoft.EntityFrameworkCore;

namespace BossBot;

public static class MigrationEvent
{
    private class TableInfo
    {
        public int? cid { get; set; }
        public string? name { get; set; }
        public string? type { get; set; }
        public int? notnull { get; set; }
        public string? dflt_value { get; set; }
        public int? pk { get; set; }
    }

    public static void Migration_1(EventInfoDataSource context)
    {
        var columns = context.Database
            .SqlQueryRaw<TableInfo>("PRAGMA table_info(EventInformationDBModels);")
            .ToList();

        var hasOneTimeEvent = columns.Any(c => c.name == "IsOneTimeEvent");
        var hasTimeBeforeNotification = columns.Any(c => c.name == "TimeBeforeNotification");

        if (!hasOneTimeEvent || !hasTimeBeforeNotification)
        {
            context.Database.ExecuteSqlRaw(
                "ALTER TABLE EventInformationDBModels RENAME TO EventInformationDBModels_old;");

            context.Database.ExecuteSqlRaw(@"
                CREATE TABLE EventInformationDBModels (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Time DATETIME NOT NULL,
                    EventName TEXT NOT NULL,
                    Days INTEGER NOT NULL,
                    ChatId INTEGER NOT NULL,
                    IsOneTimeEvent BOOLEAN NOT NULL DEFAULT 0,
                    TimeBeforeNotification INTEGER NOT NULL DEFAULT 5
                );
            ");
            context.Database.ExecuteSqlRaw(@"
                INSERT INTO EventInformationDBModels (Id, Time, EventName, Days, ChatId)
                SELECT Id, Time, EventName, Days, ChatId
                FROM EventInformationDBModels_old;
            ");
            context.Database.ExecuteSqlRaw("DROP TABLE EventInformationDBModels_old;");
        }
    }
}