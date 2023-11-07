using Luval.GPT.Chatbot.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Data
{
    public class ChatRepository
    {
        private IChatDbContext _chatDbContext;

        public ChatRepository(IChatDbContext chatDbContext)
        {
            _chatDbContext = chatDbContext;
        }

        public async void PersistRollingChat(ChatMessage chatMessage, CancellationToken cancellationToken = default)
        {
            var chatCount = _chatDbContext.ChatMessages.Count(i => i.UserId == chatMessage.UserId);

            if(chatCount > 10) await RemoveFirstChat(chatMessage.UserId, cancellationToken);

            _chatDbContext.ChatMessages.Add(chatMessage);
            await _chatDbContext.SaveChangesAsync(cancellationToken);
        }

        public async void PersistChatJob(ChatJob chatJob, CancellationToken cancellationToken = default)
        {
            var item = _chatDbContext.ChatJobs.FirstOrDefault(i => i.UserId == chatJob.UserId && i.JobName == chatJob.JobName);
            if(item != null)
            {
                item.UtcUpdatedOn = DateTime.UtcNow;
                item.Prompt = chatJob.Prompt;
                item.ChronExpression = chatJob.ChronExpression;
            }
            else
                _chatDbContext.ChatJobs.Add(chatJob);

            await _chatDbContext.SaveChangesAsync(cancellationToken);
        }

        public IEnumerable<ChatJob> GetChatJobs()
        {
            return _chatDbContext.ChatJobs;
        }

        public IEnumerable<ChatJob> GetChatJobsByUserId(string userId)
        {
            return _chatDbContext.ChatJobs.Where(i => i.UserId == userId);
        }

        public ChatJob? GetChatJob(string userId, string name)
        {
            return _chatDbContext.ChatJobs.FirstOrDefault(i => i.UserId == userId && i.JobName == name);
        }

        public IEnumerable<ChatMessage> GetHistory(string userId, string chatType = "Standard")
        {
            return _chatDbContext.ChatMessages.Where(i => i.UserId == userId && i.ChatType == chatType).OrderBy(i => i.Id);
        }

        public async Task DeleteHistoryAsync(string userId, string chatType)
        {
            _chatDbContext.ChatMessages.RemoveRange(_chatDbContext.ChatMessages.Where(i => i.UserId == userId && i.ChatType == chatType));
            await _chatDbContext.SaveChangesAsync();
        }

        public List<ValidUser> GetUsers()
        {
            return _chatDbContext.ValidUsers.ToList();
        }

        public bool IsUserValid(string userId)
        {
            return _chatDbContext.ValidUsers.Any(i => i.UserId == userId);
        }

        private async Task RemoveFirstChat(string userId, CancellationToken cancellationToken = default)
        {
            var first = _chatDbContext.ChatMessages.First();
            _chatDbContext.ChatMessages.Remove(first);
            await _chatDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
