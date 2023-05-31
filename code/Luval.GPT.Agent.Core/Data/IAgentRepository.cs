using Luval.GPT.Agent.Core.Model;

namespace Luval.GPT.Agent.Core.Data
{
    public interface IAgentRepository
    {
        SessionActivity CreateActivity(SessionActivity activity);
        Model.Agent CreateAgent(Model.Agent agent);
        Model.Agent GetAgentByCode(string agentCode);
        Session CreateSession(Model.Agent agent);
        SessionActivity UpdateActivity(SessionActivity activity);
        Session UpdateSession(Session session);

        int ResetData();
    }
}