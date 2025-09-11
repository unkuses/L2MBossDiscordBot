using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using BossBot.Options;
using OpenAI.Chat;
using CommonLib.Helpers;

namespace BossBot.Service;

public class OpenAIService
{
    private readonly ChatClient _chatClient;
    private readonly DateTimeHelper _dateTimeHelper;
    private readonly CosmoDb _cosmoDb;
    public OpenAIService(BotOptions options, CosmoDb cosmoDb, DateTimeHelper dateTimeHelper)
    {
        //_dateTimeHelper = dateTimeHelper;
        //_cosmoDb = cosmoDb;
        //var credential = new AzureKeyCredential(options.openAIKey);
        //var azureClient = new AzureOpenAIClient(new Uri(options.OpenAIEnpoint), credential);
        //_chatClient = azureClient.GetChatClient("o4-mini");
    }

    public async Task<string> GetEventResponseAsync(string prompt)
    {
        // Create a list of chat messages
        List<ChatMessage> messagesQueue =
        [
            ChatMessage.CreateAssistantMessage(prompt)
        ];
        messagesQueue.AddRange(CreateSystemMessageForEvent());

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

    private List<ChatMessage> CreateSystemMessageForEvent() =>
        [
            ChatMessage.CreateSystemMessage(@"Response only in Json"),
            ChatMessage.CreateSystemMessage(@"Event json {
                                                                  ""Event"": ""Add"",
                                                                  ""EventCommand"": ""/add_event""
                                                                }"),
            ChatMessage.CreateSystemMessage("Event can be: Add, Remove, All"),
            ChatMessage.CreateSystemMessage(@"EventCommand - One of the fallowing object in JSON format"),
            ChatMessage.CreateSystemMessage(@"RepeatAt is Day when to repeat : None = 0,
                                                                    Sun = 1 << 0, // Sunday
                                                                    Mon = 1 << 1, // Monday
                                                                    Tue = 1 << 2, // Tuesday
                                                                    Wed = 1 << 3, // Wednesday
                                                                    Thu = 1 << 4, // Thursday
                                                                    Fri = 1 << 5, // Friday
                                                                    Sat = 1 << 6  // Saturday"),
            ChatMessage.CreateSystemMessage(@"TimeBeforeNotification: time before notification should show, default 10."),
            ChatMessage.CreateSystemMessage(@"If user want to add new event use format: {
                                                                                                  ""RepeatAt"": ""Mon,Wed,Fri"",
                                                                                                  ""Time"": ""2025-07-12T14:30:00"",
                                                                                                  ""Description"": ""Recurring event on Monday, Wednesday, and Friday"",
                                                                                                  ""TimeBeforeNotification"": 15
                                                                                                }"),
            ChatMessage.CreateSystemMessage(@"If user want to remove event: { ""EventNumber"": 123"), 
            ChatMessage.CreateSystemMessage($"Current Data: {_dateTimeHelper.CurrentTime.ToLongDateString()}"),
        ];

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