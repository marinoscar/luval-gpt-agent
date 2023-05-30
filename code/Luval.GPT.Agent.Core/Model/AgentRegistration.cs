using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Model
{
    public class AgentRegistration
    {
        [JsonProperty("configurations")]
        public List<AgentConfig> Configurations { get; set; } = new List<AgentConfig>();
    }
}
