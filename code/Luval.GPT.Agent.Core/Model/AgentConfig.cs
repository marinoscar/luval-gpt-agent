using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Model
{
    public class AgentConfig
    {
        public AgentConfig()
        {
            InputParameters = new Dictionary<string, string>();
        }


        [JsonProperty("assemblyName")]
        public string? AssemblyName { get; set; }
        [JsonProperty("typeName")]
        public string? TypeName { get; set; }

        [JsonProperty("inputParameters")]
        public Dictionary<string, string> InputParameters { get; set; }

    }
}
