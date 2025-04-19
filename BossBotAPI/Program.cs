using BossBotAPI;
using CommonLib.Helpers;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton(_ => new DateTimeHelper(builder.Configuration["TimeZone"]));
builder.Services.AddSingleton(provider =>
{
    var imageAnalyzeUrl = builder.Configuration["ImageAnalyzeUrl"];
    var imageAnalyzeKey = builder.Configuration["ImageAnalyzeKey"];
    return new ImageWork(provider.GetRequiredService<CosmoDb>(), provider.GetRequiredService<DateTimeHelper>(),
        imageAnalyzeUrl, imageAnalyzeKey);
});
builder.Services.AddSingleton(provider =>
{
    var configuration = builder.Configuration;
    var endpoint = configuration["CosmosDbEndpoint"];
    var accountKey = configuration["CosmosDbAccountKey"];
    return new CosmoDb(
        provider.GetRequiredService<DateTimeHelper>(),
        endpoint,
        accountKey
    );
});

builder.Build().Run();
