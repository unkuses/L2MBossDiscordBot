using BossBot.Services.Database;
using BossBot.Services.Services;
using BossBotAPI;
using CommonLib.Helpers;
using CommonLib.Options;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddDbContextFactory<BossDbContext>(options =>
    options.UseSqlServer(GetRequiredSetting("AzureSql")));
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddSingleton(new DateTimeHelperOptions { TimeZone = GetRequiredSetting("TimeZone") });
builder.Services.AddSingleton<DateTimeHelper>();
builder.Services.AddSingleton<PlayersActivityService>();
builder.Services.AddSingleton(provider =>
{
    var imageAnalyzeUrl = GetRequiredSetting("ImageAnalyzeUrl");
    var imageAnalyzeKey = GetRequiredSetting("ImageAnalyzeKey");
    return new ImageWork(provider.GetRequiredService<CosmoDb>(), provider.GetRequiredService<DateTimeHelper>(),
        imageAnalyzeUrl, imageAnalyzeKey);
});
builder.Services.AddSingleton(provider =>
{
    var configuration = builder.Configuration;
    var endpoint = GetRequiredSetting("CosmosDbEndpoint");
    var accountKey = GetRequiredSetting("CosmosDbAccountKey");
    return new CosmoDb(
        provider.GetRequiredService<DateTimeHelper>(),
        endpoint,
        accountKey
    );
});

builder.Build().Run();

string GetRequiredSetting(string name) =>
    builder.Configuration[name] ?? throw new InvalidOperationException($"Required configuration '{name}' is missing.");
