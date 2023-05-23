using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public abstract class BaseAgent : IAgent
    {
        public BaseAgent(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Code = GetType().Name;
            InputParameters = new Dictionary<string, string>();
            Result = new Dictionary<string, string>();
        }

        /// <inheritdoc/>
        public string Code { get; protected set; }

        /// <inheritdoc/>
        public string Name { get; protected set; }

        /// <inheritdoc/>
        public string Description { get; protected set; }

        /// <inheritdoc/>
        public ILogger Logger { get; protected set; }

        /// <inheritdoc/>
        public Dictionary<string, string> InputParameters { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, string> Result { get; protected set; }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            Logger?.LogInformation($"Starting Agent: {Name}");
            await OnExecuteAsync();
            sw.Stop();
            Logger?.LogInformation($"Completed Agent: {Name} Duration: {sw.Elapsed}");
        }

        protected abstract Task OnExecuteAsync();




    }
}
