using NetOpenAI.Models;
using NetOpenApi.Api.Models;
using OpenAI_API;
using OpenAI_API.Chat;
using System.Net.Security;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
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
                chat.RequestParameters.Temperature = 0.5;
                chatRequest.SessionId = Guid.NewGuid().ToString();
                Conversations.Add(chatRequest.SessionId, chat);
            } 
            else
            {
                chat = Conversations[chatRequest.SessionId];
            }

            chat.AppendUserInput(chatRequest.RequestMessage);

            var response = await ProcessOpenAIResponse(chat);

            if (ContainsJson(response))
            {
                var json = ExtractJsonFromText(response);
                var printers = JsonSerializer.Deserialize<List<PrinterEntity>>(json);
                var emptyFields = new List<string>();
                foreach (var printer in printers)
                {
                    emptyFields = printer.GetEmptyFieldNames();
                }

                if (emptyFields.Any())
                {
                    chat.AppendSystemMessage("Ask user about the empty fields to provide the information.");
                    response = await ProcessOpenAIResponse(chat);
                }
                else
                {
                    chat.AppendSystemMessage("Tell user that you that you have successfully processed his request. And ask him if he needs any more help");

                    await CreatePrinterToHCP(printers);

                    return new ChatResponse
                    {
                        FollowupMessage = await ProcessOpenAIResponse(chat),
                        ParsedModel = ContainsJson(response) ? ExtractJsonFromText(response) : string.Empty,
                        RequestFullyParsed = ContainsJson(response),
                        SessionId = chatRequest.SessionId
                    };
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

        async Task<string> ProcessOpenAIResponse(Conversation chat, int numberOfRetries = 1)
        {
            for (int currentTry = 0; currentTry < numberOfRetries; currentTry++)
            {
                try
                {
                    return await chat.GetResponseFromChatbotAsync();
                }
                catch(Exception e)
                {
                    // TODO: log
                }
            }

            return string.Empty; // TODO: handle
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
            int startIndex = text.IndexOf("[");
            int endIndex = text.LastIndexOf("]");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                // Extract the JSON block from the text
                string json = text.Substring(startIndex, endIndex - startIndex + 1);
                return json;
            }

            return string.Empty;
        }

        async Task CreatePrinterToHCP(List<PrinterEntity> printers)
        {
            using (var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = IgnoreSslErrors }))
            {
                try
                {
                    httpClient.DefaultRequestHeaders.Add("X-Api-Key", File.ReadAllText("api-key-hcp.txt"));
                    httpClient.DefaultRequestHeaders.Add("X-Account-Domain", "hound.127.0.0.1.nip.io");

                    foreach (var printer in printers)
                    {
                        var content = new FormUrlEncodedContent(printer.GetFormParams());
                        var response = await httpClient.PutAsync("https://hound.127.0.0.1.nip.io:7300/api/v1/outputports", content);
                        response.EnsureSuccessStatusCode();
                        var responseContent = await response.Content.ReadAsStringAsync();
                    }
                }
                catch(Exception e)
                {

                }
                
            }
        }

        private bool IgnoreSslErrors(HttpRequestMessage request, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Allow all SSL certificate errors
            return true;
        }
    }
}
