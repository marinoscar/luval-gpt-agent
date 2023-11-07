using Luval.GPT.Chatbot.Channels;
using Luval.GPT.Chatbot.Channels.Telegram;
using Luval.GPT.Chatbot.Data;
using Luval.GPT.Chatbot.Data.MySql;
using Luval.GPT.Chatbot.LLM;
using Luval.GPT.Chatbot.LLM.Agents;
using Luval.GPT.Chatbot.Telegram;
using Luval.GPT.Chatbot.Telegram.Services;
using Luval.Logging.Providers;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using Telegram.Bot;

namespace Luval.GPT.Chatbot
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var logger = new CompositeLogger(new List<ILogger> { new ColorConsoleLogger(), new FileLogger() });
            var chatEndpoint = ChatEndpoint.CreateOpenAI(new ApiAuthentication(new NetworkCredential("", PrivateConfig.OpenAIKey).SecurePassword));
            var aiProvider = new GPTProvider(chatEndpoint);
            var db = new MySqlChatDbContext(PrivateConfig.DbConnection);
            InitializeDb(db);

            logger.LogInformation("Starting service");

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {

                    logger.LogInformation("Configurating services");

                    // Register Bot configuration
                    services.Configure<BotConfiguration>(
                        context.Configuration.GetSection(BotConfiguration.Configuration));

                    // Register named HttpClient to benefits from IHttpClientFactory
                    // and consume it with ITelegramBotClient typed client.
                    // More read:
                    //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-5.0#typed-clients
                    //  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
                    services.AddHttpClient("telegram_bot_client")
                            .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                            {
                                TelegramBotClientOptions options = new(PrivateConfig.TelegramKey);
                                return new TelegramBotClient(options, httpClient);
                            });

                    services.AddSingleton<IChatDbContext>(db);
                    services.AddScoped<ChatRepository>();


                    services.AddSingleton<GPTProvider>(aiProvider);
                    services.AddSingleton<ILogger>(logger);
                    services.AddScoped<IChatbotAgent, StandardChatbotAgent>();
                    services.AddScoped<UpdateHandler>();
                    services.AddScoped<ReceiverService>();
                    services.AddScoped<GPTService>();
                    services.AddScoped<ScheduleJobAgent>();
                    services.AddScoped<IChatChannelClient, TelegramChatChannelClient>();
                    services.AddHostedService<PollingService>();

                })
                .Build();

            logger.LogInformation("Running services");
            host.Run();
        }

        private static void InitializeDb(ChatDbContext dbContext)
        {
            var isCreated = dbContext.Database.EnsureCreated();
            if (isCreated)
            {
                dbContext.Database.Migrate();
                _ = dbContext.SeedDataAsync().Result;
            }
        }
    }
}