using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot
{
    public class ChatRepository
    {

        private static List<ChatMessage> messages = new List<ChatMessage>();

        public ChatMessage Add(ChatMessage chat)
        {
            chat.Id = messages.Count + 1;
            messages.Add(chat);
            return chat;
        }

        public List<ChatMessage> GetByUserId(string userId)
        {
            return messages.Where(i => i.UserId == userId).ToList();
        }

        public List<ChatMessage> GetByChatId(string chatId)
        {
            return messages.Where(i => i.ChatId == chatId).ToList();
        }
    }
}
