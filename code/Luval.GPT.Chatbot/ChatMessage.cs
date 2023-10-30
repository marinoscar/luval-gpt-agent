using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot
{
    public class ChatMessage
    {
        public long Id { get; set; }
        public string ChatId { get; set; }
        public string UserId { get; set; }
        public bool IsChatResponse { get; set; }
        public DateTime UtcDateTime { get; set; }
        public string Text { get; set; }
        public string MessageData { get; set; }
    }
}
