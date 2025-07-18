﻿using BossBot.DBModel;
using Microsoft.EntityFrameworkCore;

namespace BossBot.DataSource;

public class EventInfoDataSource : DbContext
{
    private readonly string _dbPath;
    public DbSet<EventInformationDBModel> EventInformationDbModels { get; set; }

    public EventInfoDataSource()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        _dbPath = Path.Join(path, "Events.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }
}