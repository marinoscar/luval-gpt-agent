using Luval.OpenAI.Completion;
using Luval.OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class PromptFromFileActivity : CompletionActivity
    {
        public PromptFromFileActivity(CompletionEndpoint endpoint, FileInfo file, Model model, double temperature = 0.7) : base(endpoint, File.ReadAllText(file.FullName), model, temperature)
        {

        }

        public override string Name => "Prompt From File";

        public override string Description => "Runs a prompt from a file";


        public override bool ImplementListResult => false;
    }
}
