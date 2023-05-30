using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Activity
{
    public class ActivityStatusChangeEventArgs : EventArgs
    {
        public ActivityStatusChangeEventArgs(ExecutionStatus status)
        {
            Status = status;
        }

        public ExecutionStatus Status { get; private set; }
    }
}
