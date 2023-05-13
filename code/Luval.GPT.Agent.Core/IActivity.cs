using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public interface IActivity
    {
        /// <summary>
        /// The unique name for the command
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description for the command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// The input parameters required to complete the command
        /// </summary>
        IDictionary<string, string> InputParameters { get; }

        /// <summary>
        /// Execute the command
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Key pair values coming from the exeuction
        /// </summary>
        IDictionary<string, string> Result { get; }

        /// <summary>
        /// A list with the key pair values from the execution
        /// </summary>
        List<Dictionary<string, string>> ResultList { get; }

        /// <summary>
        /// Indicates if the command implements the <see cref="ResultList"/> property
        /// </summary>
        bool ImplementListResult { get; }

        /// <summary>
        /// Gets the number of tokens used by the command
        /// </summary>
        int TokensUsed { get; }


    }
}
