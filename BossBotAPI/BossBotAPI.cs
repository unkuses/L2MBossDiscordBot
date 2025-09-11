using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using CommonLib.Requests;

namespace BossBotAPI;

public class BossBotApi(ILogger<BossBotApi> logger, ImageWork imageWork)
{
    [Function("ParseImage")]
    public async Task<IActionResult> ParseImage([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            // Parse the request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestData>(requestBody);

            if (requestData == null || string.IsNullOrEmpty(requestData.TimeZone) || requestData.Image == null)
            {
                return new BadRequestObjectResult("Invalid request. Please provide both TimeZone and Image.");
            }

            var result = await imageWork.ProcessImage(requestData.Image, requestData.ChatId, requestData.TimeZone, requestData.Language);
            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("ParseImageByUrl")]
    public async Task<IActionResult> ParseImageURL([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request.");

        try
        {
            // Parse the request body
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestParseImageUrl>(requestBody);

            if (requestData == null || string.IsNullOrEmpty(requestData.Url))
            {
                return new BadRequestObjectResult("Invalid request. Please provide both TimeZone and Image.");
            }

            var result = await imageWork.ProcessImageByUrl(requestData.Url, requestData.ChatId, requestData.TimeZone, requestData.Language);
            // Log the received data
            logger.LogInformation($"Received TimeZone: {requestData.TimeZone}");
            logger.LogInformation($"Received Image url: {requestData.Url.Length}");


            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("GetStatisticInfo")]
    public async Task<IActionResult> GetStatisticInfo([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req, string chatId)
    {
        logger.LogInformation("C# HTTP trigger function processed a request for statistics.");
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestParseImageUrl>(requestBody);
            var users = await imageWork.GetBossStatistic(requestData.Url);
            return new OkObjectResult(users);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}