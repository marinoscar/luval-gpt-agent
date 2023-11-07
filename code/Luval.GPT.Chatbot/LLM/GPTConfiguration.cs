using Luval.GPT.Chatbot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.LLM
{
    public class GPTConfiguration
    {
        public GPTConfiguration()
        {
            ChatMessages = new List<ChatMessage>();
        }

        public string IncomingMessage { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
    }
}
