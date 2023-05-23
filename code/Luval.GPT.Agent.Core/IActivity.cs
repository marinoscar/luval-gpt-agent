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
        /// Unique code for the activity
        /// </summary>
        string Code { get; }

        /// <summary>
        /// The unique name for the command
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Description for the command
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets or sets the max number of retries
        /// </summary>
        int MaxRetries { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before retrying
        /// </summary>
        TimeSpan DelayBetweenRetries { get; set; }

        /// <summary>
        /// Gets the status of the activity
        /// </summary>
        ExecutionStatus Status { get; }

        /// <summary>
        /// The input parameters required to complete the command
        /// </summary>
        Dictionary<string, string> InputParameters { get; set; }

        /// <summary>
        /// Execute the command
        /// </summary>
        Task ExecuteAsync();

        /// <summary>
        /// Key pair values coming from the exeuction
        /// </summary>
        Dictionary<string, string> Result { get; }

        /// <summary>
        /// A list with the key pair values from the execution
        /// </summary>
        List<Dictionary<string, string>> ResultList { get; }

        /// <summary>
        /// Indicates if the command implements the <see cref="ResultList"/> property
        /// </summary>
        bool ImplementListResult { get; }

        /// <summary>
        /// Event on the case of an error
        /// </summary>
        event EventHandler<ActivityErrorEventArgs> ActivityError;

        /// <summary>
        /// Event when the activity sends a message to the subscriber
        /// </summary>
        event EventHandler<ActivityMessageEventArgs> ActivityMessage;

        /// <summary>
        /// Event when the activity status has changed
        /// </summary>
        event EventHandler<ActivityStatusChangeEventArgs> ActivityStatusChanged;

        /// <summary>
        /// Event when the activity is starting
        /// </summary>
        event EventHandler ActivityStarting;

        /// <summary>
        /// Event when the activity is completed
        /// </summary>
        event EventHandler ActivityCompleted;


    }

    /// <summary>
    /// Indicates the status of an run
    /// </summary>
    public enum ExecutionStatus { 
        /// <summary>
        /// No status provided
        /// </summary>
        None, 
        /// <summary>
        /// Activity is pending of starting
        /// </summary>
        Pending, 
        /// <summary>
        /// Activity is in progress
        /// </summary>
        InProgress, 
        /// <summary>
        /// Activity was completed
        /// </summary>
        Completed, 
        /// <summary>
        /// Activity has faulted
        /// </summary>
        Faulted,
        /// <summary>
        /// Activity is retrying
        /// </summary>
        Retrying
    }
}
