using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.Channels.Telegram
{
    public static class Extensions
    {
        public static ChatTextMessage ToChatTextMessage(this Message m)
        {
            return new ChatTextMessage()
            {
                ChatId = m.Chat.Id.ToString(),
                UserId = m.From?.Id.ToString(),
                MessageId = m.MessageId.ToString(),
                FirstName = m.From.FirstName,
                LastName = m.From.LastName,
                UserName = m.From.Username,
                Date = m.Date,
                Text = m.Text
            };
        }

        public static IChatChannelClient ToChatClient(this ITelegramBotClient c)
        {
            return new TelegramChatChannelClient(c);
        }
    }
}
