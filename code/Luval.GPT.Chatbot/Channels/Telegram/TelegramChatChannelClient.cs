using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;

namespace Luval.GPT.Chatbot.Channels.Telegram
{
    public class TelegramChatChannelClient : IChatChannelClient
    {

        readonly ITelegramBotClient _client;

        public TelegramChatChannelClient(ITelegramBotClient client)
        {
            _client = client;
        }

        public async Task<ChatTextMessage> SendTextMessageAsync(string chatId, string message, CancellationToken cancellationToken)
        {
            return (await _client.SendTextMessageAsync(
                        chatId: Convert.ToInt64(chatId),
                        text: message,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken)).ToChatTextMessage();
        }
    }
}
