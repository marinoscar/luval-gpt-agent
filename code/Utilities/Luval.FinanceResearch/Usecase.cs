using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Luval.FinanceResearch
{
    public class Usecase
    {
        [JsonIgnore]
        public string? FinanceArea { get; set; }
        [JsonIgnore]
        public string? SubProcess { get; set; }
        [JsonIgnore]
        public string? SubProcessDescription { get; set; }
        [JsonIgnore]
        public string? KeyPerformanceIndicator { get; set; }
        [JsonIgnore]
        public string? KPIDescription { get; set; }

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
    }
}
