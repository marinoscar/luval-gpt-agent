using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public static class ModelInputs
    {
        public static List<string> Sectors { get; private set; } = new List<string>()
        { "Industrial Products", "Chemicals and Advance Materials", "Automotive",
          "Transportation", "Consumer Products", "Retail", "Power and Utilities", "Oil and Gas",
          "Goverment Public Sector","Life Sciences", "Health",
          "Technology","Media and Entertainment", "Telecomunications"};

        public static List<string> Capabilities { get; private set; } = new List<string>() 
        { "Code Generations, Optimization, Completion", 
          "Content Generation", 
          "Knowledge management for search, training and next best action" };

        public static string GetCapabilitiesText()
        {
            return string.Join(Environment.NewLine, Capabilities);
        }
    }
}
