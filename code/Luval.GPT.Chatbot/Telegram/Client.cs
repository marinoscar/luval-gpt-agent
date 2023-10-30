using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Luval.GPT.Chatbot.Telegram
{
    public class Client : IDisposable
    {
        private TelegramBotClient _botClient;
        private ILogger _logger;
        CancellationTokenSource _cts;

        public Client(string key, ILogger logger)
        {
            _botClient = new TelegramBotClient(key);
            _logger = logger;
        }

        public void Dispose()
        {
            Stop();
        }

        public async Task<User> Start()
        {
            _logger.LogInformation("Starting chatbot");
            _cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
            };

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: _cts.Token
            );

            async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
            {
                // Only process Message updates: https://core.telegram.org/bots/api#message
                if (update.Message is not { } message)
                    return;
                // Only process text messages
                if (message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                _logger.LogDebug($"Received a '{messageText}' message in chat {chatId}.");

                // Echo received message text
                var echoMessage = "You said:\n" + messageText;
                Message sentMessage = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: echoMessage,
                    cancellationToken: cancellationToken);

                _logger.LogDebug($"Sent echo message: {echoMessage}");
            }

            Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
            {
                var ErrorMessage = exception switch
                {
                    ApiRequestException apiRequestException
                        => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                    _ => exception.ToString()
                };

                _logger.LogError(ErrorMessage);
                return Task.CompletedTask;
            }

            var me = await _botClient.GetMeAsync();

            return me;
            
        }

        public void Stop()
        {
            if(_cts != null) _cts.Cancel();
        }

    }
}
