using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public static class Prompts
    {

        public static string GetValueChain(string sector)
        {
            return GetContent("ValueChainSteps").Replace("{sector}", sector);
        }

        public static string GetValueChainStepSamples(string sector, string step)
        {
            return GetContent("ValueChainExamples")
                .Replace("{sector}", sector)
                .Replace("{step}", step)
                .Replace("{capabilities}", GetCapabilities());
        }

        public static string GetCapabilities()
        {
            return GetContent("OpenAI-Capabilities");
        }

        private static string GetContent(string promptFileName)
        {
            if (string.IsNullOrWhiteSpace(promptFileName)) return null;
            if (!promptFileName.EndsWith(".txt")) promptFileName += ".txt";
            return File.ReadAllText(string.Format("{0}\\Prompts\\{1}", Environment.CurrentDirectory, promptFileName));
        }
    }
}
