using Luval.GPT.Agent.Core.Activity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.MeetingNotes.Activities
{
    public class WriteReportActivity : BaseActivity
    {

        public WriteReportActivity(ILogger logger) : base(logger)
        {
            Name = "Create an HTML report";
            Description = Name;
        }

        public override string Name { get; set; }
        public override string Description { get; set; }

        public override bool ImplementListResult => false;

        protected override Task OnExecuteAsync()
        {
            if(!IsValid()) return Task.CompletedTask;

            var jsonContent = InputParameters["JsonResult"];


            return Task.Run(() =>
            {
                DoCreateFile(jsonContent, GetReportLocation());
            });


        }

        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(InputParameters["JsonResult"]))
            {
                Logger.LogWarning("Missing Input Paramater JsonResult");
                return false;
            }
            if (string.IsNullOrWhiteSpace(InputParameters["DestinationFolder"]))
            {
                Logger.LogWarning("Missing Input Paramater DestinationFolder");
                return false;
            }
            if (string.IsNullOrWhiteSpace(InputParameters["AudioFile"]))
            {
                Logger.LogWarning("Missing Input Paramater AudioFile");
                return false;
            }
            return true;
        }

        private FileInfo GetReportLocation()
        {
            var file = new FileInfo(InputParameters["AudioFile"]);
            var folder = InputParameters["DestinationFolder"];
            var fileName = file.Name.Replace(file.Extension, "-report.html");
            return new FileInfo(Path.Combine(folder, fileName));
        }

        private void DoCreateFile(string jsonContent, FileInfo file)
        {
            var result = JsonConvert.DeserializeObject<AnalyzerResult>(jsonContent);
            var writer = new HtmlWriter(file, result.Subject);
            writer.AddHeading(result.Subject, 1);
            writer.AddHeading("Summary", 2);
            writer.AddParragraph(GetContent(result.Summary));
            writer.AddHeading("Action Items", 2);
            writer.AddUnOrderedList(GetContent(result.ActionItems).Split(Environment.NewLine).Where(i => !string.IsNullOrWhiteSpace(i)));
            writer.AddHeading("Transcript", 2);
            writer.AddParragraph(GetContent(result.Transcript));
            writer.Save();
        }

        private string GetContent(string fileName)
        {
            if (!File.Exists(fileName)) return string.Empty;
            return File.ReadAllText(fileName);
        }
    }
}
