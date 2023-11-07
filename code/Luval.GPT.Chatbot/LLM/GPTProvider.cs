using Luval.GPT.Chatbot.Data.Entities;
using Luval.OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.LLM
{
    public class GPTProvider
    {
        private ChatEndpoint _chatEndpoint;

        public GPTProvider(ChatEndpoint chatEndpoint)
        {
            _chatEndpoint = chatEndpoint;
        }

        public async Task<string> RespondToMessageAsync(IEnumerable<ChatMessage> history, string incoming)
        {
            _chatEndpoint.ClearMessages();
            foreach (ChatMessage chatMessage in history)
            {
                _chatEndpoint.AddUserMessage(chatMessage.UserText);
                _chatEndpoint.AddAssitantMessage(chatMessage.AgentText);
            }
            _chatEndpoint.AddUserMessage(incoming);
            var response = await _chatEndpoint.SendAsync();
            _chatEndpoint.AddAssitantMessage(response.Choice.Message.Content);
            return response.Choice.Message.Content;
        }
    }
}
