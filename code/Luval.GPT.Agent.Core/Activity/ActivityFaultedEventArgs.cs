using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core.Activity
{
    public class ActivityFaultedEventArgs : EventArgs
    {

        public ActivityFaultedEventArgs(Exception ex)
        {
            Exception = ex;
        }
        public Exception Exception { get; set; }
    }
}
