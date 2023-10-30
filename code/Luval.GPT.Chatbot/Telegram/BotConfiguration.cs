using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Telegram
{
    public class BotConfiguration
    {

        public BotConfiguration()
        {
            BotToken = PrivateConfig.TelegramKey;
        }

        public static readonly string Configuration = "BotConfiguration";

        public string BotToken { get; set; } = "";
    }
}
