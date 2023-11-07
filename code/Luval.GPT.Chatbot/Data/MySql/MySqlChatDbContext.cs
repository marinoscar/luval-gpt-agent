using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Data.MySql
{
    public class MySqlChatDbContext : ChatDbContext
    {
        private string _connectionString;

        public MySqlChatDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            object value = optionsBuilder.UseMySQL(_connectionString);
        }
    }
}
