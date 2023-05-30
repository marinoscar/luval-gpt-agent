using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Activity
{
    public class ActivityErrorEventArgs : EventArgs
    {
        public ActivityErrorEventArgs(Exception exception, int retryCount, int totalRetry)
        {
            Exception = exception;
            RetryCount = retryCount;
            TotalRetry = totalRetry;
            CancelRetry = false;
        }

        /// <summary>
        /// Gets the <see cref="Exception"/>
        /// </summary>
        public Exception? Exception { get; private set; }

        /// <summary>
        /// Gets the count of retry
        /// </summary>
        public int RetryCount { get; private set; }

        /// <summary>
        /// Gets the total number of retries
        /// </summary>
        public int TotalRetry { get; private set; }

        /// <summary>
        /// Indicates if a retry needs to be cancelled
        /// </summary>
        public bool CancelRetry { get; set; }
    }
}
