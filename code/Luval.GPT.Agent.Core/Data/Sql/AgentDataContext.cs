using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luval.GPT.Agent.Core.Model;
using System.Runtime.CompilerServices;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Luval.GPT.Agent.Core.Data.Sql
{
    public class AgentDataContext : DbContext
    {
        private string _connectionString;
        public AgentDataContext() : this(GetConStr())
        {

        }

        public AgentDataContext(string connString) : base()
        {
            _connectionString = connString;

            var dbCreator = (RelationalDatabaseCreator)Database.GetService<IDatabaseCreator>();
            dbCreator.EnsureCreated();
        }

        private static string GetConStr()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "Data", "Agent.mdf");
            var conn = @$"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={file};Integrated Security=True";
            return conn;
        }

        public DbSet<Model.Agent> Agents { get; set; }
        public DbSet<Session> Sessions { get; set; }

        public DbSet<SessionActivity> SessionActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Session>()
                .HasOne(p => p.Agent)
                .WithMany()
                .HasForeignKey(p => p.AgentId);

            modelBuilder.Entity<SessionActivity>()
                .HasOne(p => p.Session)
                .WithMany()
                .HasForeignKey(p => p.SessionId);

        }

        

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connection => { });
            optionsBuilder.UseSqlServer(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
