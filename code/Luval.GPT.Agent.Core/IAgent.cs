using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public interface IAgent
    {

        /// <summary>
        /// Unique code for the agent
        /// </summary>
        string Code { get; }

        /// <summary>
        /// The unique name for the agent
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description for the agent
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the logger for the agent
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// The input parameters required to complete the command
        /// </summary>
        Dictionary<string, string> InputParameters { get; set; }

        /// <summary>
        /// Key pair values coming from the exeuction
        /// </summary>
        Dictionary<string, string> Result { get; }

        /// <summary>
        /// Execute the command
        /// </summary>
        Task ExecuteAsync();

    }
}
