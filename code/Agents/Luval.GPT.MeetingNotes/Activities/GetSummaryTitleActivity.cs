using Luval.GPT.Agent.Core.Activity;
using Luval.OpenAI.Chat;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.MeetingNotes.Activities
{
    public class GetSummaryTitleActivity : ChatActivity
    {
        public GetSummaryTitleActivity(ILogger logger, ChatEndpoint endpoint, string prompt) : base(logger, endpoint, prompt, 0d)
        {
            Name = "Extract Title";
        }
    }
}
