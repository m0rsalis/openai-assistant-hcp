using OpenAI_API;
using OpenAI_API.Chat;

namespace NetOpenAI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            OpenAIAPI api = new OpenAIAPI(await File.ReadAllTextAsync("api-key.txt"));

            var chat = api.Chat.CreateConversation();

            /// give instruction as System
            chat.AppendSystemMessage(await File.ReadAllTextAsync("context.txt"));

            // give a few examples as user and assistant
            //chat.AppendUserInput("Is this an animal? Cat");
            //chat.AppendExampleChatbotOutput("Yes");
            //chat.AppendUserInput("Is this an animal? House");
            //chat.AppendExampleChatbotOutput("No");

            // now let's ask it a question'
            chat.AppendUserInput("I would like to install two fujifilm printers, one with ip 10.23.2.23 and serial number ff_1 and the other one with ip 127.0.0.1 and serial number ff_2. Both of them should print via PCL6. Users should be able to login with card or pin");
            // and get the response
            string response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response); // "Yes"

            // and continue the conversation by asking another
            chat.AppendUserInput("Is this an animal? Chair");
            // and get another response
            response = await chat.GetResponseFromChatbotAsync();
            Console.WriteLine(response); // "No"

            // the entire chat history is available in chat.Messages
            foreach (ChatMessage msg in chat.Messages)
            {
                Console.WriteLine($"{msg.Role}: {msg.Content}");
            }
        }
    }
}