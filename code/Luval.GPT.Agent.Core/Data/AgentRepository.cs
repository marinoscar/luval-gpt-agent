using Luval.GPT.Agent.Core.Data.Sql;
using Luval.GPT.Agent.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Luval.GPT.Agent.Core.Data
{
    public class AgentRepository : IAgentRepository
    {


        public AgentRepository() : this(string.Empty) { }
        public AgentRepository(string connString)
        {
            if (!string.IsNullOrEmpty(connString))
                Context = new AgentDataContext(connString);
            else Context = new AgentDataContext();
        }

        public AgentDataContext Context { get; set; }

        public Model.Agent CreateAgent(Model.Agent agent)
        {
            Context.Agents.Add(agent);
            Context.SaveChanges();
            return agent;
        }

        public Model.Agent GetAgentByCode(string agentCode)
        {
            return Context.Agents.FirstOrDefault(i => i.Code == agentCode);
        }

        public Session CreateSession(Model.Agent agent)
        {
            var s = new Session() { Agent = agent, AgentId = agent.Id };
            Context.Sessions.Add(s);
            Context.SaveChanges();
            return s;
        }

        public Session UpdateSession(Session session)
        {
            session.UtcModifiedOn = DateTime.UtcNow;
            Context.Sessions.Update(session);
            Context.SaveChanges();
            return session;
        }

        public SessionActivity CreateActivity(SessionActivity activity)
        {
            activity.Version = 1;
            activity.UtcModifiedOn = DateTime.UtcNow;
            Context.SessionActivities.Add(activity);
            Context.SaveChanges();
            return activity;
        }

        public SessionActivity UpdateActivity(SessionActivity activity)
        {
            activity.Version += 1;
            activity.UtcModifiedOn = DateTime.UtcNow;
            Context.SessionActivities.Update(activity);
            Context.SaveChanges();
            return activity;
        }

    }
}
