using Luval.OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot
{
    public class AIProvider
    {
        private ChatEndpoint _chatEndpoint;
        private ChatRepository _chatRepository;

        public AIProvider(ChatEndpoint chatEndpoint, ChatRepository chatRepository)
        {
            _chatEndpoint = chatEndpoint;
            _chatRepository = chatRepository;
        }

        public async Task<string> RespondToMessageAsync(string incoming)
        {
            _chatEndpoint.AddUserMessage(incoming);
            var response = await _chatEndpoint.SendAsync();
            _chatEndpoint.AddAssitantMessage(response.Choice.Message.Content);
            return response.Choice.Message.Content;
        }
    }
}
