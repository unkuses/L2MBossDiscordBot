using System.Text;
using Azure;
using Azure.AI.Vision.ImageAnalysis;
using CommonLib.Helpers;

namespace BossBotAPI
{
    public class ImageWork
    {
        private readonly CosmoDb _bossData;
        private readonly DateTimeHelper _dateTimeHelper;
        private readonly string _imageAnalysisUrl;
        private readonly string _imageAnalysisKey;

        public ImageWork(CosmoDb bossData, DateTimeHelper dateTimeHelper, string imageAnalysisUrl, string imageAnalysisKey)
        {
            _bossData = bossData;
            _dateTimeHelper = dateTimeHelper;
            _imageAnalysisUrl = imageAnalysisUrl;
            _imageAnalysisKey = imageAnalysisKey;
        }

        public async Task<string> ProcessImageByUrl(string url, ulong chatId, string timeZone)
        {
            return await ProcessImageInternal(url, null, chatId, timeZone);
        }

        public async Task<string> ProcessImage(byte[] image, ulong chatId, string timeZone)
        {
            return await ProcessImageInternal(null, image, chatId, timeZone);
        }

        private async Task<string> ProcessImageInternal(string? url, byte[]? image, ulong chatId, string timeZone)
        {
            // Read text from Azure using the appropriate input (URL or byte array)
            var bossInformationList = url != null
                ? await ReadTextAzure(url)
                : await ReadTextAzure(image!);

            // Parse the boss information
            var bossList = await _bossData.ImageAnalyzeParser(bossInformationList, chatId, timeZone, image is null);

            // Build the response string
            var stringBuilder = new StringBuilder();
            foreach (var bossModel in bossList)
            {
                var nextRespawnTime = bossModel.KillTime.AddHours(bossModel.RespawnTime);
                var timeToRespawn = nextRespawnTime - _dateTimeHelper.CurrentTime;
                stringBuilder.AppendLine(
                    $"Босс убит **{bossModel.Id}** **{bossModel.NickName.ToUpper()}** респавн {nextRespawnTime:HH:mm} через {timeToRespawn:hh\\:mm}");
            }

            return stringBuilder.ToString();
        }

        private async Task<List<string>> ReadTextAzure(string url)
        {
            return await ReadTextAzureInternal(new Uri(url), null);
        }

        private async Task<List<string>> ReadTextAzure(byte[] image)
        {
            return await ReadTextAzureInternal(null, BinaryData.FromBytes(image));
        }

        private async Task<List<string>> ReadTextAzureInternal(Uri? url, BinaryData? imageData)
        {
            var text = new List<string>();
            var client = new ImageAnalysisClient(new Uri(_imageAnalysisUrl), new AzureKeyCredential(_imageAnalysisKey));

            ImageAnalysisResult result;
            if (url != null)
            {
                result = await client.AnalyzeAsync(url, VisualFeatures.Read, new ImageAnalysisOptions { GenderNeutralCaption = true });
            }
            else if (imageData != null)
            {
                result = await client.AnalyzeAsync(imageData, VisualFeatures.Read, new ImageAnalysisOptions { GenderNeutralCaption = true });
            }
            else
            {
                throw new ArgumentException("Either URL or image data must be provided.");
            }

            // Extract text from the analysis result
            foreach (var block in result.Read.Blocks)
            {
                text.AddRange(block.Lines.Select(line => line.Text));
            }

            return text;
        }
    }
}
