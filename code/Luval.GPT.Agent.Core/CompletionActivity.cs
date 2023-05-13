using Luval.OpenAI;
using Luval.OpenAI.Completion;
using Luval.OpenAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class CompletionActivity : IActivity
    {


        public CompletionActivity(CompletionEndpoint endpoint, string prompt, double temperature = 0.7d) : this(endpoint, prompt, Model.TextDavinci003, temperature)
        {

        }

        protected CompletionActivity(CompletionEndpoint endpoint, string prompt, Model model, double temperature = 0.7d)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));

            Completion = endpoint;
            InputParameters = new Dictionary<string, string>();
            Temperature = temperature;
            Prompt = prompt;
            Model = model;
            Result = new Dictionary<string, string>();
            ResultList = new List<Dictionary<string, string>>();
        }


        /// <summary>
        /// Gets the command prompt
        /// </summary>
        protected virtual string Prompt { get; private set; }

        /// <summary>
        /// Gets the command temperature
        /// </summary>
        protected virtual double Temperature { get; private set; }

        /// <summary>
        /// Gets the model to use for the prompt
        /// </summary>
        protected virtual Model Model { get; private set; }

        /// <summary>
        /// Gets a completion endpoint
        /// </summary>
        protected CompletionEndpoint Completion { get; private set; }

        /// <inheritdoc/>
        public virtual string Name => "Completion Activity";

        /// <inheritdoc/>
        public virtual string Description => "Completes a prompt";

        /// <inheritdoc/>
        public IDictionary<string, string> InputParameters { get; private set; }

        /// <inheritdoc/>
        public IDictionary<string, string> Result { get; private set; }

        /// <inheritdoc/>
        public virtual List<Dictionary<string, string>> ResultList { get; private set; }

        /// <inheritdoc/>
        public virtual bool ImplementListResult => false;

        /// <inheritdoc/>
        public virtual int TokensUsed { get; private set; }

        /// <inheritdoc/>
        public virtual async Task ExecuteAsync()
        {
            var p = ApplyParametersToPrompt();
            var tokens = TokenCalculator.FromPrompt(p);
            var result = default(CompletionResponse);
            try
            {
                result = await Completion.SendAsync(p, tokens, Model, Temperature);
            }
            catch (Exception ex )
            {
                throw new Exception($"Failed to run prompt: {p}", ex);
            }
            if (result.Usage != null) TokensUsed += result.Usage.TotalTokens;
            if (result.Choice != null)
            {
                Result["choice"] = result.Choice.Text;
            }
        }

        /// <summary>
        /// Updates the prompt with the input parameters
        /// </summary>
        /// <returns>The prompt with the required parameters</returns>
        protected virtual string ApplyParametersToPrompt()
        {
            var result = Prompt;
            foreach (var p in InputParameters)
            {
                result = result.Replace("{" + p.Key + "}", p.Value);
            }
            return result;
        }
    }
}
