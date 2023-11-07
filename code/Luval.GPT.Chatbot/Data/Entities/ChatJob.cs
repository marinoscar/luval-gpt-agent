using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace Luval.GPT.Chatbot.Data.Entities
{
    [Index(nameof(UserId)), Index(nameof(JobCode))]
    public class ChatJob
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [Required, MaxLength(50)]
        public string? ChatId { get; set; }
        [Required, MaxLength(50)]
        public string? UserId { get; set; }

        [Required, MaxLength(100)]
        public string? JobCode { get; set; }

        [Required, MaxLength(75)]
        public string? JobName { get; set; }

        [Required, MaxLength(100)]
        public string? ChronExpression { get; set; }

        [Required, MaxLength(1000)]
        public string? Prompt { get; set; }

        [Required, MaxLength(1000)]
        public DateTime UtcUpdatedOn { get; set; }

        public static ChatJob Create(GPTScheduleJson json, Message message)
        {
            return new ChatJob()
            {
                ChatId = message.Chat.Id.ToString(),
                UserId = message.From.Id.ToString(),
                ChronExpression = json.Chron,
                Prompt = json.Prompt,
                JobName = json.Name,
                JobCode = GetCode(message.From.Id.ToString(), json.Name),
                UtcUpdatedOn = DateTime.UtcNow
            };
        }

        public string GetCode()
        {
            if(!string.IsNullOrWhiteSpace(JobCode)) return JobCode;
            return GetCode(UserId, JobName);
        }

        private static string GetCode(string userId, string name)
        {
            return userId + "_" + name.ToLowerInvariant().Replace(" ", "_");
        }

    }
}
