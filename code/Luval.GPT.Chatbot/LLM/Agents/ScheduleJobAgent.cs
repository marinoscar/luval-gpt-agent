using Luval.GPT.Chatbot.Data;
using Luval.GPT.Chatbot.Data.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.LLM.Agents
{
    public class ScheduleJobAgent : ChatbotAgentBase
    {
        public ScheduleJobAgent(GPTService gptService, ChatRepository chatRepository, ILogger logger) : base("SYS-Schedule-Job", gptService, chatRepository, logger)
        {

        }

        public override async Task<Message> OnResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var res = await base.OnResponse(botClient, message, cancellationToken);
            if (!string.IsNullOrWhiteSpace(res.Text) && res.Text.Contains("JOB_IS_DONE"))
                PersistJob(res.Text, message);
            return res;
        }

        private void PersistJob(string content, Message message)
        {
            var jsonObject = JsonConvert.DeserializeObject<GPTScheduleJson>(content);
            var item = ChatJob.Create(jsonObject, message);
            Repository.PersistChatJob(item);
        }
    }
}
