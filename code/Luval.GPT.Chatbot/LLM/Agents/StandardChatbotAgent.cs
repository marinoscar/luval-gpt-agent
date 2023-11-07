using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;
using Luval.GPT.Chatbot.Data;
using Luval.GPT.Chatbot.Data.Entities;
using Luval.GPT.Chatbot.LLM;
using System.Diagnostics;
using Luval.GPT.Chatbot.Telegram;

namespace Luval.GPT.Chatbot.LLM.Agents
{
    public class StandardChatbotAgent : ChatbotAgentBase
    {
        public StandardChatbotAgent(GPTService gptService, ChatRepository chatRepository, ILogger logger) : base("SYS-Standard", gptService, chatRepository, logger)
        {
            
        }

    }
}
