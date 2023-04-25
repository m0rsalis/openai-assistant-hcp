using NetOpenAI.Models;
using NetOpenApi.Api.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace NetOpenApi.Api
{
    public class OpenAiConnector
    {
        OpenAIAPI api;

        public OpenAiConnector()
        {
            api = new OpenAIAPI(File.ReadAllText("api-key.txt"));
        }

        private static Dictionary<string, Conversation> Conversations = new Dictionary<string, Conversation>();

        public async Task<ChatResponse> ProcessChatRequest(Models.ChatRequest chatRequest)
        {
            Conversation chat;

            if (string.IsNullOrEmpty(chatRequest.SessionId) || !Conversations.ContainsKey(chatRequest.SessionId))
            {
                chat = api.Chat.CreateConversation();
                var context = await File.ReadAllTextAsync("context.txt");

                foreach (var field in FieldDefinitions.Definitions)
                {
                    context += field.ToString();
                }

                /// give instruction as System
                chat.AppendSystemMessage(context);
                chatRequest.SessionId = Guid.NewGuid().ToString();
                Conversations.Add(chatRequest.SessionId, chat);
            } 
            else
            {
                chat = Conversations[chatRequest.SessionId];
            }

            chat.AppendUserInput(chatRequest.RequestMessage);

            var response = await chat.GetResponseFromChatbotAsync();

            if (ContainsJson(response))
            {
                var json = ExtractJsonFromText(response);
                var printer = JsonSerializer.Deserialize<PrinterEntity>(json);
                var emptyFields = printer.GetEmptyFieldNames();

                if (emptyFields.Any())
                {
                    chat.AppendSystemMessage("Ask user to provide info for following fields: " + string.Join(",", emptyFields));
                    response = await chat.GetResponseFromChatbotAsync();
                }
            }

            return new ChatResponse
            {
                FollowupMessage = response,
                ParsedModel = ContainsJson(response) ? ExtractJsonFromText(response) : string.Empty,
                RequestFullyParsed = ContainsJson(response),
                SessionId = chatRequest.SessionId
            };
        }

        static bool ContainsJson(string text)
        {
            return text.Contains("{") && text.Contains("}");
        }

        static string ExtractJsonFromText(string text)
        {
            if (text.StartsWith("{") && text.EndsWith("}"))
            {
                return text;
            }

            // Search for JSON block enclosed in triple backticks
            int startIndex = text.IndexOf("{");
            int endIndex = text.LastIndexOf("}");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                // Extract the JSON block from the text
                string json = text.Substring(startIndex, endIndex - startIndex + 1);
                return json;
            }

            return string.Empty;
        }
    }
}
