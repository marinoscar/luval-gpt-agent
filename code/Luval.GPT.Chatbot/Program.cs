using Luval.GPT.Chatbot.Telegram;
using Luval.Logging.Providers;

namespace Luval.GPT.Chatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var chatbot = new Client(ConfigReader.Get("telegramKey"), new ColorConsoleLogger());
            var user = chatbot.Start().Result;
            Console.WriteLine($"Start listening for @{user.Username}");
            Console.ReadLine();

            // Send cancellation request to stop bot
            chatbot.Stop();
        }
    }
}