using Luval.GPT.Agent.Core.Activity;
using Luval.GPT.Agent.Core.Data;
using Luval.GPT.Agent.Core.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        public BaseAgent(ILogger logger, IAgentRepository agentRepository)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Repository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));

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

        /// <summary>
        /// Gets the repository to persist the data
        /// </summary>
        public IAgentRepository Repository { get; protected set; }

        /// <summary>
        /// Gets the agent data entity
        /// </summary>
        public Model.Agent Agent { get; protected set; }

        /// <summary>
        /// Gets the session entity
        /// </summary>
        public Session Session { get; protected set; }


        /// <inheritdoc/>
        public Dictionary<string, string> InputParameters { get; set; }

        /// <inheritdoc/>
        public Dictionary<string, string> Result { get; protected set; }

        /// <inheritdoc/>
        public async Task ExecuteAsync()
        {
            var sw = Stopwatch.StartNew();
            InitializeAgent();
            Logger?.LogInformation($"Starting Agent: {Name}");
            Session.Status = ExecutionStatus.InProgress;
            Repository.UpdateSession(Session);

            try
            {
                await OnExecuteAsync();
                Session.Status = ExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                Session.Status = ExecutionStatus.Faulted;
                Session.ErrorMessage = ex.Message;
                Logger?.LogError(ex, $"{Name} Faulted");
            }

            Session.DurationInSeconds = sw.Elapsed.TotalSeconds;
            Session.UtcStoppedOn = DateTime.UtcNow;
            Session.Result = JsonConvert.SerializeObject(Result);
            Repository.UpdateSession(Session);

            sw.Stop();
            Logger?.LogInformation($"Completed Agent: {Name} Duration: {sw.Elapsed}");

        }

        protected virtual void InitializeAgent()
        {
            Agent = Repository.GetAgentByCode(Code);
            if (Agent == null)
            {
                var a = new Model.Agent()
                {
                    Name = Name,
                    Code = Code,
                    Description = Description,
                    FullQualifiedName = GetType().FullName,
                    InputParameters = JsonConvert.SerializeObject(InputParameters)
                };
                Agent = Repository.CreateAgent(a);
            }

            var session = Repository.CreateSession(Agent);
            Session = session;
        }

        protected abstract Task OnExecuteAsync();

        protected async virtual Task RunActivity(IActivity activity)
        {
            var a = new SessionActivity()
            {
                Session = Session,
                SessionId = Session.Id,
                Name = activity.Name,
                Code = activity.Code,
                InputParameters = JsonConvert.SerializeObject(InputParameters),
                Status = ExecutionStatus.InProgress,
                UtcStartedOn = DateTime.UtcNow
            };

            activity.ActivityStatusChanged += (s, e) =>
            {
                a.Status = e.Status;
                Repository.UpdateActivity(a);
            };

            activity.ActivityError += (s, e) =>
            {
                a.Status = ExecutionStatus.Retrying;
                a.RetryCount = e.RetryCount;
                Repository.UpdateActivity(a);
            };

            activity.ActivityFaulted += (s, e) =>
            {
                a.Status = ExecutionStatus.Faulted;
                a.ErrorMessage = e.ToString();
                Repository.UpdateActivity(a);

            };

            Repository.CreateActivity(a);

            await activity.ExecuteAsync();
            a.UtcCompletedOn = DateTime.UtcNow;
            a.DurationInSeconds = a.UtcCompletedOn.Value.Subtract(a.UtcStartedOn.Value).TotalSeconds;
            a.Status = ExecutionStatus.Completed;
            a.Result = JsonConvert.SerializeObject(activity.Result);
            Repository.UpdateActivity(a);

        }




    }
}
