using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using CommonLib.Helpers;

namespace BossBot
{
    public class OpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly DateTimeHelper _dateTimeHelper;
        public OpenAIService(Options options, DateTimeHelper dateTimeHelper)
        {
            _dateTimeHelper = dateTimeHelper;
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
    }
}