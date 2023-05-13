using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class ValueChain
    {
        [JsonIgnore]
        public string? Sector { get; set; }

        [JsonIgnore]
        public int? Step { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("challenge")]
        public string? Challenge { get; set; }

    }
}
