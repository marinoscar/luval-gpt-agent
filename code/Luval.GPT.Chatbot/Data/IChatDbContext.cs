using Luval.GPT.Chatbot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Luval.GPT.Chatbot.Data
{
    public interface IChatDbContext
    {
        DbSet<ChatMessage> ChatMessages { get; set; }
        DbSet<ValidUser> ValidUsers { get; set; }
        DbSet<ChatJob> ChatJobs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}