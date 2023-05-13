using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class SampleCapability
    {
        [JsonIgnore]
        public string? Sector { get; set; }

        [JsonPropertyName("capability")]
        public string? Capability { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("implementation")]
        public string? Implementation { get; set; }

        [JsonPropertyName("endpoint")]
        public string? Endpoint { get; set; }

        [JsonPropertyName("customModel")]
        public string? CustomModel { get; set; }

        [JsonIgnore]
        public ValueChain? Step { get; set; }

    }
}
