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

        public virtual async Task<Message> OnResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            return await OnTimedResponse(botClient, message, cancellationToken);
        }

        protected virtual async Task<Message> OnTimedResponse(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var result = default(Message);
            var resultMessage = await ControlledExecution(async () => { return await GetResponse(message); },
                async () =>
                {
                    await SendResponse(botClient, message, "Please hold, I'm working on your response", cancellationToken);
                });


            result = await SendResponse(botClient, message, resultMessage, cancellationToken);

            return result;
        }

        protected virtual Task<Message> SendResponse(ITelegramBotClient botClient, Message message, string text, CancellationToken cancellationToken)
        {
            _logger.LogDebug($"To: {message.From?.Id} Response: {text}");
            return botClient.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: cancellationToken);
        }

        private async Task<string> GetResponse(Message message)
        {
            if (!Repository.IsUserValid(message.From.Id.ToString()))
                return $"{message.From.FirstName} thanks for trying to use this bot, please contact the administrator and provide this number {message.From.Id} to get your user activated";

            var history = Repository.GetHistory(message.From.Id.ToString(), ChatType);

            var config = new GPTConfiguration() { ChatMessages = history.ToList(), IncomingMessage = message.Text };
            _gptService.Configuration = config;

            var response = await _gptService.ExecuteAsync();
            var chatMessage = ChatMessage.FromTelegram(message, ChatType);

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
