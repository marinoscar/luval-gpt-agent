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
        readonly ILogger _logger;

        public TelegramChatChannelClient(ITelegramBotClient client, ILogger logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<ChatTextMessage> SendTextMessageAsync(string chatId, string message, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"ChatID: {chatId}\nMessage:{message}");
            var r = default(ChatTextMessage);
            try
            {
                r = (await _client.SendTextMessageAsync(
                        chatId: Convert.ToInt64(chatId),
                        text: message,
                        replyMarkup: new ReplyKeyboardRemove(),
                        cancellationToken: cancellationToken)).ToChatTextMessage();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex, "Failed to send the message");
                throw ex;
            }
            return r;
        }
    }
}
