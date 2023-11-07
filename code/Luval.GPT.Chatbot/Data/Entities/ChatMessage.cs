using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Luval.GPT.Chatbot.Data.Entities
{
    [Index(nameof(UserId)), Index(nameof(ChatType))]
    public class ChatMessage
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, MaxLength(50)]
        public string? ChatId { get; set; }
        [Required, MaxLength(50)]
        public string? UserId { get; set; }

        [Required, MaxLength(100)]
        public string? ChatType { get; set; }

        [Required]
        public DateTime DateTime { get; set; }
        [Required]
        public string? UserText { get; set; }
        [Required]
        public string? AgentText { get; set; }
        [Required]
        public string? MessageData { get; set; }

        public static ChatMessage FromTelegram(Message message, string chatType = "Standard")
        {
            if(message == null) throw new ArgumentNullException(nameof(message));
            return new ChatMessage()
            {
                ChatId = message.Chat.Id.ToString(),
                UserId = message.From?.Id.ToString(),
                UserText = message.Text,
                DateTime = message.Date,
                MessageData = JsonConvert.SerializeObject(message)
            };
        }
    }
}
