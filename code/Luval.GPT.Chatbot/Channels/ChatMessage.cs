using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.Channels
{
    public class ChatTextMessage
    {
        public string? ChatId { get; set; }
        public string? UserId { get; set; }
        public string? Text { get; set; }
        public DateTime Date { get; set; }
    }
}
