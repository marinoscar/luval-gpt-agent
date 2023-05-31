using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.MeetingNotes
{
    public class TranscriptAnalyzerResult
    {
        public string Transcript { get; set; }
        public string Summary { get; set; }
        public string ActionItems { get; set; }

        public string Subject { get; set; }
    }
}
