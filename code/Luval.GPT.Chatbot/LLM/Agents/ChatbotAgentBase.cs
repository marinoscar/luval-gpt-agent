using Luval.GPT.Chatbot.Data.Entities;
using Luval.GPT.Chatbot.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using Luval.GPT.Chatbot.Telegram;
using Luval.GPT.Chatbot.Channels;

namespace Luval.GPT.Chatbot.LLM.Agents
{
    public class ChatbotAgentBase : IChatbotAgent
    {
        private readonly GPTService _gptService;
        private readonly ILogger _logger;
        protected virtual ChatRepository Repository { get; private set; }

        protected string ChatType { get; private set; }

        protected ChatbotAgentBase(string chatType, GPTService gptService, ChatRepository chatRepository, ILogger logger)
        {
            _gptService = gptService;
            _logger = logger;
            Repository = chatRepository;
            ChatType = chatType;
        }

        public virtual async Task<ChatTextMessage> OnResponse(IChatChannelClient client, ChatTextMessage message, CancellationToken cancellationToken)
        {
            return await OnTimedResponse(client, message, cancellationToken);
        }

        protected virtual async Task<ChatTextMessage> OnTimedResponse(IChatChannelClient client, ChatTextMessage message, CancellationToken cancellationToken)
        {
            var result = default(ChatTextMessage);
            var resultMessage = await ControlledExecution(async () => { return await GetResponse(message); },
                async () =>
                {
                    await SendResponse(client, message, "Please hold, I'm working on your response", cancellationToken);
                });


            result = await SendResponse(client, message, resultMessage, cancellationToken);

            return result;
        }

        protected virtual Task<ChatTextMessage> SendResponse(IChatChannelClient client, ChatTextMessage message, string text, CancellationToken cancellationToken)
        {
            return client.SendTextMessageAsync(message.ChatId, text, cancellationToken);
        }

        private async Task<string> GetResponse(ChatTextMessage message)
        {
            if (!Repository.IsUserValid(message.UserId))
                return $"{message.FirstName} thanks for trying to use this bot, please contact the administrator and provide this number {message.UserId} to get your user activated";

            var history = Repository.GetHistory(message.UserId, ChatType);

            var config = new GPTConfiguration() { ChatMessages = history.ToList(), IncomingMessage = message.Text };
            _gptService.Configuration = config;

            var response = await _gptService.ExecuteAsync();
            var chatMessage = ChatMessage.FromChatClient(message, ChatType);

            chatMessage.AgentText = response.Result;
            Repository.PersistRollingChat(chatMessage);

            return response.Result ?? "";
        }

        private Task<string> ControlledExecution(Func<Task<string>> doAI, Action sendMessage, double waitInMs = 20000)
        {
            var lastMessage = false;
            var stopwatch = new Stopwatch();
            var task = doAI();
            stopwatch.Start();
            while (!task.IsCompleted)
            {
                if (stopwatch.ElapsedMilliseconds > waitInMs && !lastMessage)
                {
                    sendMessage();
                    lastMessage = true;
                }
                Task.Delay(1000).Wait();
            }
            task.Wait();
            stopwatch.Stop();
            return task;
        }
    }
}
