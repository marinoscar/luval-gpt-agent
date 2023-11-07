using Telegram.Bot;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.Telegram
{
    public interface IChatbotAgent
    {
        Task<Message> OnResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}