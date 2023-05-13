using Luval.OpenAI.Completion;
using Luval.OpenAI.Models;
using Luval.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Luval.OpenAI.Chat;
using Microsoft.Extensions.Logging;

namespace Luval.GPT.Agent.Core
{
    public class ChatActivity : IActivity
    {
        public ChatActivity(ILogger logger, ChatEndpoint endpoint, string prompt, double temperature = 0.7d) : this(logger, endpoint, prompt, Model.TextDavinci003, temperature)
        {

        }

        protected ChatActivity(ILogger logger, ChatEndpoint endpoint, string prompt, Model model, double temperature = 0.7d)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (model == null) throw new ArgumentNullException(nameof(model));
            if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));

            Logger = logger;
            Chat = endpoint;
            InputParameters = new Dictionary<string, string>();
            Temperature = temperature;
            Prompt = prompt;
            Model = model;
            Result = new Dictionary<string, string>();
            ResultList = new List<Dictionary<string, string>>();
        }


        protected virtual ILogger Logger { get; private set; }

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
        protected ChatEndpoint Chat { get; private set; }

        /// <inheritdoc/>
        public virtual string Name => "Chat Activity";

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
            var result = default(ChatResponse);
            var tries = 0;
            var success = false;
            while (!success)
            {
                success = true;
                try
                {
                    Chat.AddUserMessage(p);
                    result = await Chat.SendAsync(Temperature);
                    Chat.ClearMessages();
                }
                catch (Exception ex)
                {
                    success = false;
                    var message = $"Failed to run prompt: {p}";
                    Logger.LogWarning($"FAILED TRY {tries+1}");
                    Logger.LogWarning($"{message} with exception: {ex}");
                    if (tries >= 3)
                    {
                        success = true;
                        Logger.LogError($"ERROR: UNABLE TO COMPLETE AFTER {tries}");
                    }
                    tries++;
                }
            }
            if (result != null)
            {
                if (result?.Usage != null) TokensUsed += result.Usage.TotalTokens;
                if (result?.Choice != null)
                {
                    Result["choice"] = result.Choice.ToString().Replace("```json", "").Replace("```", "");
                }
            }
            else
                Result["choice"] = string.Empty;
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
