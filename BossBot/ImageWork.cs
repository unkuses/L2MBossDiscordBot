using System.Text;
using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace BossBot;

public class ImageWork(BossData bossData, DateTimeHelper dateTimeHelper, Options options)
{
    public async Task<string> ProcessImage(string url, ulong chatId, ulong usedId)
    {
        var bossInformationList = await ReadTextAzure(url);
        var bossList = bossData.ImageAnalyzeParser(bossInformationList, usedId, chatId);
        var stringBuilder = new StringBuilder();
        foreach (var bossModel in bossList)
        {
            var nextRespawnTime = bossModel.KillTime.AddHours(bossModel.RespawnTime);
            var timeToRespawn = nextRespawnTime - dateTimeHelper.CurrentTime;
            stringBuilder.AppendLine(
                $"Босс убит **{bossModel.Id}** **{bossModel.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn.ToString(@"hh\:mm")}");
        }
        return stringBuilder.ToString();
    }
    
    private async Task<List<string>> ReadTextAzure(string url)
    {
        var text = new List<string>();
        var client = new ImageAnalysisClient(new Uri(options.ImageAnalysisUrl), new AzureKeyCredential(options.ImageAnalysisKey));
        ImageAnalysisResult result = await client.AnalyzeAsync(new Uri(url), VisualFeatures.Read,
            new ImageAnalysisOptions { GenderNeutralCaption = true });
        // Send image to Azure Computer Vision OCR endpoint
        foreach (var block in result.Read.Blocks)
        {
            text.AddRange(block.Lines.Select(line => line.Text));
        }

        return text;
    }
}