using Luval.GPT.Agent.Core.Activity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.MeetingNotes.Activities
{
    public class WriteReportActivity : BaseActivity
    {

        public WriteReportActivity(ILogger logger): base(logger)
        {
            Name = "Create a report";
            Description = Name;
        }

        public override string Name { get; set; }
        public override string Description { get; set; }

        public override bool ImplementListResult => false;

        protected override Task OnExecuteAsync()
        {

            throw new NotImplementedException();
        }
    }
}
