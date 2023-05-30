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
using Newtonsoft.Json;

namespace Luval.GPT.Agent.Core.Activity
{
    public class ChatActivity : BaseActivity, ILLMActivity
    {
        public ChatActivity(ILogger logger, ChatEndpoint endpoint, string prompt, double temperature = 0.7d) : base(logger)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));
            if (string.IsNullOrEmpty(prompt)) throw new ArgumentNullException(nameof(prompt));

            Logger = logger;
            Chat = endpoint;
            InputParameters = new Dictionary<string, string>();
            Temperature = temperature;
            Prompt = prompt;
            Result = new Dictionary<string, string>();
            ResultList = new List<Dictionary<string, string>>();
            Name = "Chat Activity";
            Description = "Runs a prompt";
        }

        #region Property Implementation

        /// <summary>
        /// Gets the command prompt
        /// </summary>
        protected virtual string Prompt { get; private set; }

        /// <summary>
        /// Gets the command temperature
        /// </summary>
        protected virtual double Temperature { get; private set; }

        /// <summary>
        /// Gets a completion endpoint
        /// </summary>
        protected virtual ChatEndpoint Chat { get; private set; }

        /// <inheritdoc/>
        public int TokensUsed { get; protected set; }

        /// <inheritdoc/>
        public override string Name { get; set; }

        /// <inheritdoc/>
        public override string Description { get; set; }

        /// <inheritdoc/>
        public override bool ImplementListResult => false;

        #endregion

        #region Methods

        /// <summary>
        /// Runs the prompt
        /// </summary>
        protected async override Task OnExecuteAsync()
        {
            var p = ApplyParametersToPrompt();
            var result = default(ChatResponse);
            Chat.ClearMessages();
            Chat.AddUserMessage(p);
            LogDebug($"[{Name}] - Running Prompt: {p}");
            result = await Chat.SendAsync(Temperature);
            Chat.ClearMessages();
            if (result != null)
            {
                if (result?.Usage != null) TokensUsed += result.Usage.TotalTokens;
                if (result?.Choice != null)
                {
                    foreach (var choice in result.Choices)
                    {
                        choice.Message.Content = CleanUpResponse(choice);
                    }
                    Result["choice"] = result.Choice.ToString();
                }
            }
            else
                Result["choice"] = string.Empty;
        }

        /// <summary>
        /// Casts the result into an object
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>A casted result</returns>
        public virtual T CastResult<T>()
        {
            if (Result == null || !Result.Values.Any()) return default;
            var content = Result.Values.FirstOrDefault();
            if (string.IsNullOrEmpty(content)) return default;
            return JsonConvert.DeserializeObject<T>(content);
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

        /// <summary>
        /// Method is run for all of the choices as part of the chat response
        /// </summary>
        /// <param name="chatChoice">The response coming from the model</param>
        /// <returns>A cleand up version of the choice</returns>
        protected virtual string CleanUpResponse(ChatChoice chatChoice)
        {
            return chatChoice.ToString().Replace("```json", "").Replace("```", "");
        }

        #endregion

    }
}
