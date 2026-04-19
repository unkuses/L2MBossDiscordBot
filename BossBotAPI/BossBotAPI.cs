using BossBot.Services.Services;
using CommonLib.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BossBotAPI;

public class BossBotApi(ILogger<BossBotApi> logger, PlayersActivityService playersActivityService, ImageWork imageWork)
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

            var result = await imageWork.ProcessImage(requestData.Image, requestData.ChatId, requestData.TimeZone,
                requestData.Language);
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

            var result = await imageWork.ProcessImageByUrl(requestData.Url, requestData.ChatId, requestData.TimeZone,
                requestData.Language);
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

    [Function("AddUsers")]
    public async Task<IActionResult> GetStatisticInfo(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request for statistics.");
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestAddUser>(requestBody);
            var users = await imageWork.GetBossStatistic(requestData.Url);
            var result = await playersActivityService.AddNewPlayerName(requestData.ChatId, users);
            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("CleanUsers")]
    public async Task<IActionResult> CleanUsers([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequest req,
        ulong chatId)
    {
        logger.LogInformation("C# HTTP trigger function processed a request for cleaning users.");
        try
        {
            await playersActivityService.CleanUsers(chatId);
            return new OkObjectResult("Users cleaned successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("AddUserToEvents")]
    public async Task<IActionResult> AddUserToEvents([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request for adding users to events.");
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestAddUserToEvent>(requestBody);
            var users = await imageWork.GetBossStatistic(requestData.Url);
            var result = await playersActivityService.PopulatePlayersActivityAsync(requestData.ChatId, users,
                requestData.EventName);
            return new OkObjectResult($"Users {string.Join(" ,", result)} Added to the {requestData.EventName}.");
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("GetStatisticForEvent")]
    public async Task<IActionResult> GetStatisticForEvent([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        logger.LogInformation("C# HTTP trigger function processed a request for getting statistics for an event.");
        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var requestData = JsonSerializer.Deserialize<RequestGetStatistic>(requestBody);
            var result = await playersActivityService.GetUserStatisticByEventName(requestData.ChatId, requestData.EventName);
            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            logger.LogError($"Error processing request: {ex.Message}");
            return new ObjectResult(ex);
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