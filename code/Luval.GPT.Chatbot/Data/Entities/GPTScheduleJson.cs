using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.Data.Entities
{

    public class GPTScheduleJson
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("prompt")]
        public string? Prompt { get; set; }
        [JsonProperty("chron")]
        public string? Chron { get; set; }
    }
}
