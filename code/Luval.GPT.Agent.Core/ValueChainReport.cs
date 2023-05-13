using Luval.OpenAI.Chat;
using Luval.OpenAI.Completion;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Data;
using System.Diagnostics;

namespace Luval.GPT.Agent.Core
{
    public class ValueChainReport
    {

        public ILogger Logger { get; private set; }
        public ChatEndpoint Endpoint { get; private set; }
        public double Temperature { get; private set; }

        public ValueChainReport(ILogger logger, ChatEndpoint endpoint, double temperature = 0.7d)
        {
            Logger = logger;
            Endpoint = endpoint;
            Temperature = temperature;
        }

        public int TotalTokens { get; private set; }

        public void Execute(List<ValueChain> totalSteps)
        {
            var sw = new Stopwatch();
            sw.Start();
            var total = new List<SampleCapability>();
            Logger.LogInformation("Getting Capabilities");
            var count = 0d;
            double totalProgress = totalSteps.Count;
            foreach (var sector in ModelInputs.Sectors)
            {
                var steps = totalSteps.Where(i => i.Sector == sector).ToList();
                foreach (var step in steps)
                {
                    var items = GetSampleData(sector, step, ModelInputs.GetCapabilitiesText());
                    total.AddRange(items);
                    count++;
                    Logger.LogInformation($"Total Progress {((count / totalProgress) * 100).ToString("N2")}");
                    Logger.LogInformation($"Tokens used so far: {TotalTokens.ToString("N2")}");
                    Logger.LogInformation($"Duration so far: {sw.Elapsed.ToString()}");
                }
            }
            sw.Stop();
            Logger.LogInformation("Process completed");
            Logger.LogInformation($"Total used tokens: {TotalTokens.ToString("N2")}");
            Logger.LogInformation($"Total duration: {sw.Elapsed.ToString()}");
            var totalContent = JsonConvert.SerializeObject(total);
            Logger.LogDebug(totalContent);
            File.WriteAllText("result.json", totalContent);
            ToExcel(total);
        }

        public static void ToExcel(List<SampleCapability> data)
        {
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, "report.xlsx"));
            if (file.Exists) file.Delete();
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("Data");
                var first = data.First();
                var row = 2;
                sheet.Cells[1, 1].Value = "Sector";
                sheet.Cells[1, 2].Value = "Step";
                sheet.Cells[1, 3].Value = "Value Chain";
                sheet.Cells[1, 4].Value = "Value Chain Name";
                sheet.Cells[1, 5].Value = "Value Chain Description";
                sheet.Cells[1, 6].Value = "Value Chain Challenge";
                sheet.Cells[1, 7].Value = "Capability";
                sheet.Cells[1, 8].Value = "Title";
                sheet.Cells[1, 9].Value = "Description";
                sheet.Cells[1, 10].Value = "Implementation";
                sheet.Cells[1, 11].Value = "Endpoint";
                sheet.Cells[1, 12].Value = "Custom Model";

                foreach (var item in data)
                {
                    sheet.Cells[row, 1].Value = item.Sector;
                    sheet.Cells[row, 2].Value = item.Step?.Step;
                    sheet.Cells[row, 3].Value = item.Step?.Name;
                    sheet.Cells[row, 4].Value = $"{item.Step?.Step?.ToString().PadLeft(2, '0')} - {item.Step?.Name}";
                    sheet.Cells[row, 5].Value = item.Step?.Description;
                    sheet.Cells[row, 6].Value = item.Step?.Challenge;
                    sheet.Cells[row, 7].Value = item.Capability;
                    sheet.Cells[row, 8].Value = item.Title;
                    sheet.Cells[row, 9].Value = item.Description;
                    sheet.Cells[row, 10].Value = item.Implementation;
                    sheet.Cells[row, 11].Value = item.Endpoint;
                    sheet.Cells[row, 12].Value = item.CustomModel;
                    row++;
                }
                var range = sheet.Cells[1, 1, (row - 1), 12];
                var table = sheet.Tables.Add(range, "DataTable");
                // configure the table
                table.ShowHeader = true;
                table.ShowFirstColumn = true;
                range.AutoFitColumns();
                // Save to file
                package.Save();
            }
        }

        private List<SampleCapability> GetSampleData(string sector, ValueChain step, string capability)
        {
            Logger.LogInformation($"Getting samples for {sector} for the value chaing step {step.Name} and capability {capability}");
            var p = GetFileContent("ValueChainExamples")
                .Replace("{sector}", sector)
                .Replace("{step}", step.Name)
                .Replace("{challenge}", step.Challenge)
                .Replace("{capability}", capability);
            Logger.LogDebug($"Prompt: {p}");
            var activity = new ChatActivity(Logger, Endpoint, p, 0d);
            activity.ExecuteAsync().Wait();
            TotalTokens += activity.TokensUsed;
            var content = activity.Result.Values.First();
            var res = JsonConvert.DeserializeObject<List<SampleCapability>>(content);
            res?.ForEach(i =>
            {
                i.Sector = sector;
                i.Step = step;
            });

            Logger.LogDebug(content);
            return res;
        }

        private string GetFileContent(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            if (!name.EndsWith(".txt")) name += ".txt";
            return File.ReadAllText($"{Environment.CurrentDirectory}\\Prompts\\{name}");
        }
    }
}