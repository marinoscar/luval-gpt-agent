using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FinanceResearch.Inputs
{
    public static class Prompts
    {
        public static List<string> ProcessAreas = new List<string>()
        { "Financial Planning and Analysis", "Accounting and Financial Reporting", "Treasury and Cash Management",
        "Tax Planning and Compliance", "Risk Management", "Internal Audit", "Investor Relations"};

        public static List<string> Capabilities { get; private set; } = new List<string>()
        { "Code Generations, Optimization, Completion",
          "Content Generation",
          "Knowledge management for search, training and next best action" };

        public static string SubProcesses = @"
For the finance function, in {area} provide the following information
1. Name of the sub-process (JSON key: name)
2. Description of the sub-process (JSON key: description)
Provide the information as a JSON array
";
        public static string SubProcessKPI = @"
For the finance function, in {area} as part of the sub-process {name} in which {description} please provide
1. Name of a Key Performance Indicator (JSON Key: name)
2. Description of the Key Performance Indicator (JSON Key: description)
Provide the information as a JSON array
";
        public static string SubProcessKPIUseCase = @"
For the finance function, in {area} as part of the sub-process {name} in which {description}
one important Key Performance Indicator (K.P.I.) is {kpi} in which {kpi-description}
In order to have the organization improve the K.P.I. with the use of OpenAI GPT-3 capabilities

{capabilities}

Provide the following information
1. OpenAI GPT-3 capability the use case would use (JSON Key: capability)
2. Detail description of the use case, write it as an objective, do not start with To or Be (JSON Key: description)
3. Provide a description on how the use case can be implemented with Open AI GPT-3 (JSON Key: implementation)
4. Indicate if the use case requires the use of Open AI GPT-3 Emebedings, Completions or Other endpoint (JSON Key: endpoint)
5. Indicate if the use case requires the use of Open AI GPT-3 custom model, with Yes, No or N/A (JSON Key: customModel)
6. Title for the description of the use case (JSON Key: title)

Provide the information in a JSON array format and only the JSON

";
    }

}
