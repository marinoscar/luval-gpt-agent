using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Activity
{
    public abstract class BaseActivity : IActivity
    {

        private ExecutionStatus _status;

        public BaseActivity(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Code = GetType().Name;
            InputParameters = new Dictionary<string, string>();
            Result = new Dictionary<string, string>();
            ResultList = new List<Dictionary<string, string>>();
            _status = ExecutionStatus.Pending;
            MaxRetries = 0;
            DelayBetweenRetries = TimeSpan.Zero;
        }


        #region Property Implementation

        /// <inheritdoc/>
        public virtual string Code { get; private set; }

        /// <inheritdoc/>
        public abstract string Name { get; set; }

        /// <inheritdoc/>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="ILogger"/>
        /// </summary>
        public virtual ILogger Logger { get; protected set; }

        /// <inheritdoc/>
        public virtual Dictionary<string, string> InputParameters { get; set; }

        /// <inheritdoc/>
        public virtual Dictionary<string, string> Result { get; protected set; }

        /// <inheritdoc/>
        public virtual List<Dictionary<string, string>> ResultList { get; protected set; }

        /// <inheritdoc/>
        public abstract bool ImplementListResult { get; }

        /// <inheritdoc/>
        public int MaxRetries { get; set; }

        /// <inheritdoc/>
        public TimeSpan DelayBetweenRetries { get; set; }


        /// <inheritdoc/>
        public ExecutionStatus Status
        {
            get { return _status; }
            protected set
            {
                _status = value;
                ActivityStatusChanged?.Invoke(this, new ActivityStatusChangeEventArgs(value));
            }
        }

        #endregion

        #region Event Handling

        /// <inheritdoc/>
        public event EventHandler<ActivityFaultedEventArgs> ActivityFaulted;

        /// <inheritdoc/>
        public event EventHandler<ActivityErrorEventArgs> ActivityError;

        /// <inheritdoc/>
        public event EventHandler<ActivityMessageEventArgs> ActivityMessage;

        /// <inheritdoc/>
        public event EventHandler<ActivityStatusChangeEventArgs> ActivityStatusChanged;

        /// <inheritdoc/>
        public event EventHandler ActivityStarting;

        /// <inheritdoc/>
        public event EventHandler ActivityCompleted;

        protected void OnActivityError(ActivityErrorEventArgs args)
        {
            ActivityError?.Invoke(this, args);
        }

        protected void OnActivityFaulted(ActivityFaultedEventArgs args)
        {
            ActivityFaulted?.Invoke(this, args);
        }

        protected virtual void OnActivityMessage(ActivityMessageEventArgs args)
        {
            ActivityMessage?.Invoke(this, args);
        }

        protected virtual void OnActivityStarting()
        {
            ActivityStarting?.Invoke(this, new EventArgs());
        }

        protected virtual void OnActivityCompleted()
        {
            ActivityCompleted?.Invoke(this, new EventArgs());
        }

        #endregion


        /// <inheritdoc/>
        public virtual async Task ExecuteAsync()
        {
            OnActivityStarting();
            var sw = Stopwatch.StartNew();
            LogInfo($"Starting {Name}");
            var success = false;
            var retries = 0;
            Status = ExecutionStatus.InProgress;
            while (!success)
            {
                try
                {
                    await OnExecuteAsync();
                    success = true;
                    Status = ExecutionStatus.Completed;
                }
                catch (Exception ex)
                {
                    var e = new ActivityErrorEventArgs(ex, retries + 1, MaxRetries);
                    if (retries >= MaxRetries)
                    {
                        SetAsFaulted(ex);
                        return;
                    }
                    else
                    {
                        if (e.CancelRetry)
                        {
                            SetAsFaulted(ex);
                            return;
                        }
                        Status = ExecutionStatus.Retrying;
                        OnActivityError(e);
                        LogWarning($"Error running: {Name} Retry attempt number {retries + 1} Retrying after: {DelayBetweenRetries} Error: {ex}");
                        retries++;
                        await Task.Delay(DelayBetweenRetries);
                    }
                }
            }
            sw.Stop();
            Status = ExecutionStatus.Completed;
            OnActivityCompleted();
            LogInfo($"Completed {Name} in {sw.Elapsed}");
        }

        private void SetAsFaulted(Exception ex)
        {
            LogError($"Failed running {Name} Exception: {ex}");
            Status = ExecutionStatus.Faulted;
            OnActivityFaulted(new ActivityFaultedEventArgs(ex));
        }

        protected abstract Task OnExecuteAsync();

        protected virtual void LogDebug(string message)
        {
            Logger?.LogInformation(message);
            OnActivityMessage(new ActivityMessageEventArgs(LogLevel.Debug, message));
        }

        protected virtual void LogTrace(string message)
        {
            Logger?.LogTrace(message);
            OnActivityMessage(new ActivityMessageEventArgs(LogLevel.Trace, message));
        }

        protected virtual void LogInfo(string message)
        {
            Logger?.LogInformation(message);
            OnActivityMessage(new ActivityMessageEventArgs(LogLevel.Information, message));
        }

        protected virtual void LogWarning(string message)
        {
            Logger?.LogWarning(message);
            OnActivityMessage(new ActivityMessageEventArgs(LogLevel.Warning, message));
        }

        protected virtual void LogError(string message)
        {
            Logger?.LogError(message);
            OnActivityMessage(new ActivityMessageEventArgs(LogLevel.Error, message));
        }


    }
}
