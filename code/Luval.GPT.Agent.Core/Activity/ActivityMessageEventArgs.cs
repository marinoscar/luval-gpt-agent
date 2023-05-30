using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Activity
{
    public class ActivityMessageEventArgs : EventArgs
    {
        public ActivityMessageEventArgs(LogLevel logLevel, string message)
        {
            LogLevel = logLevel;
            Message = message;
        }

        /// <summary>
        /// Gets the log level
        /// </summary>
        public LogLevel LogLevel { get; private set; }

        /// <summary>
        /// Gets the message
        /// </summary>
        public string? Message { get; private set; }
    }
}
