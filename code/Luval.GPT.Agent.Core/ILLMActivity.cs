using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public interface ILLMActivity : IActivity
    {
        /// <summary>
        /// Gets the number of tokens used by the command
        /// </summary>
        int TokensUsed { get; }
    }
}
