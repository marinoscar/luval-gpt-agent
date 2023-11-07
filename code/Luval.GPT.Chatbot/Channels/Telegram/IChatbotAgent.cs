using Luval.GPT.Chatbot.Channels;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.Telegram
{
    public interface IChatbotAgent
    {
        Task<ChatTextMessage> OnResponse(IChatChannelClient client, ChatTextMessage message, CancellationToken cancellationToken);
    }
}