using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Channels
{
    public interface IChatChannelClient
    {
        Task<ChatTextMessage> SendTextMessageAsync(string chatId, string message, CancellationToken cancellationToken);
    }
}
