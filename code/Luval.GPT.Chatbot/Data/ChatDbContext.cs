using Luval.GPT.Chatbot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Data
{
    public class ChatDbContext : DbContext, IChatDbContext
    {
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<ValidUser> ValidUsers { get; set; }
        public DbSet<ChatJob> ChatJobs { get; set; }

        public virtual async Task<int> SeedDataAsync(CancellationToken cancellationToken = default)
        {
            if (ValidUsers != null && !ValidUsers.Any())
            {
                ValidUsers?.AddAsync(new ValidUser() { UserId = "5640988132" }, cancellationToken);
                ValidUsers?.AddAsync(new ValidUser() { UserId = "5235751730" }, cancellationToken);
                ValidUsers?.AddAsync(new ValidUser() { UserId = "601762548" }, cancellationToken);
            }
            return await SaveChangesAsync(cancellationToken);
        }
    }
}
