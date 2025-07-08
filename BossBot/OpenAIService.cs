using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using OpenAI.Chat;
using CommonLib.Helpers;
using Discord.Rest;

namespace BossBot;

public class OpenAIService
{
    private readonly ChatClient _chatClient;
    private readonly DateTimeHelper _dateTimeHelper;
    private readonly CosmoDb _cosmoDb;
    public OpenAIService(Options options, CosmoDb cosmoDb, DateTimeHelper dateTimeHelper)
    {
        _dateTimeHelper = dateTimeHelper;
        _cosmoDb = cosmoDb;
        var credential = new AzureKeyCredential(options.openAIKey);
        var azureClient = new AzureOpenAIClient(new Uri(options.OpenAIEnpoint), credential);
        _chatClient = azureClient.GetChatClient("o4-mini");
    }

    public async Task<string> GetEventResponseAsync(string prompt)
    {
        // Create a list of chat messages
        List<ChatMessage> messagesQueue =
        [
            ChatMessage.CreateSystemMessage(@"Response only in given format:
                                                         Days should be in format: Mon, Tue, Wed, Thu, Fri, Sat, Sun
                                                         If user want to add event for days: add Days Hours:Minutes Event Message
                                                         If user want to add event on specific date: add Month/Day:Hours:Minutes Event Message
                                                         If want to remove event: remove Event Number
                                                         If want to see all exist events: all"),

            ChatMessage.CreateSystemMessage($"Current Data: {_dateTimeHelper.CurrentTime.ToLongDateString()}"),
            ChatMessage.CreateAssistantMessage(prompt)
        ];

        try
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messagesQueue);
            if (completion != null)
            {
                return completion.Content[0].Text;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }

        return string.Empty;
    }

    public async Task<string> GetBossResponseAsync(ulong chatId, string prompt)
    {
        var bossList = await _cosmoDb.GetAllLoggedBossInfoAsync(chatId);
        var bossListJson = JsonSerializer.Serialize(bossList);
        // Create a list of chat messages
        List<ChatMessage> messagesQueue =
        [
            ChatMessage.CreateSystemMessage("Main response language Russian"),
            ChatMessage.CreateSystemMessage(@"You are a bot that provides information about bosses in the game."),
            ChatMessage.CreateSystemMessage(@"You will receive a prompt with a request for boss information."),
            ChatMessage.CreateSystemMessage(@"Good to know information: if any upcoming chains and chains location"),
            ChatMessage.CreateSystemMessage(@"Limit answer with 5000 chars"),
            ChatMessage.CreateSystemMessage(@"Usual response with 10 boss infos if there is not close bosses in 10 minute range"),
            ChatMessage.CreateSystemMessage(@"If ask for chain response should have bosses with close RespawnTime time"),
            ChatMessage.CreateSystemMessage(@"Chance output format: 33:orange_circle:, 50:yellow_circle:, 100:green_circle:"),
            ChatMessage.CreateSystemMessage(@"PurpleDrop output format: add | :eggplant: if true"),
            ChatMessage.CreateSystemMessage("for all Time provide information format: HH:mm"),
            ChatMessage.CreateSystemMessage("Try to add always provide NextRespawnTime information hrs and minutes"), 
            ChatMessage.CreateSystemMessage($"Boss list information: {bossListJson}"),
            ChatMessage.CreateSystemMessage($"Current Data: {_dateTimeHelper.CurrentTime:F}"),
            ChatMessage.CreateAssistantMessage(prompt)
        ];

        try
        {
            ChatCompletion completion = await _chatClient.CompleteChatAsync(messagesQueue);
            if (completion != null)
            {
                return completion.Content[0].Text;
            }
        }
        catch (Exception ex)
        {
            return string.Empty;
        }

        return string.Empty;
    }
}