using Luval.OpenAI.Chat;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class ValueChainStepReport
    {
        public ILogger Logger { get; private set; }
        public ChatEndpoint Endpoint { get; private set; }
        public double Temperature { get; private set; }
        public int TotalTokens { get; private set; }

        public ValueChainStepReport(ILogger logger, ChatEndpoint endpoint, double temperature = 0.7d)
        {
            Logger = logger;
            Endpoint = endpoint;
            Temperature = temperature;
        }

        public void Execute()
        {
            var sw = new Stopwatch();
            sw.Start();
            var total = new List<ValueChain>();
            Logger.LogInformation("Getting Value chain information");
            var count = 1d;
            double totalProgress = ModelInputs.Sectors.Count;
            foreach (var sector in ModelInputs.Sectors)
            {
                var steps = GetValueChains(sector);
                total.AddRange(steps);
                Logger.LogInformation($"Total Progress {((count / totalProgress) * 100).ToString("N2")}");
                Logger.LogInformation($"Tokens used so far: {TotalTokens.ToString("N2")}");
                Logger.LogInformation($"Duration so far: {sw.Elapsed.ToString()}");
                count++;
            }
            sw.Stop();
            Logger.LogInformation("Process completed");
            Logger.LogInformation($"Total used tokens: {TotalTokens.ToString("N2")}");
            Logger.LogInformation($"Total duration: {sw.Elapsed.ToString()}");
            var totalContent = JsonConvert.SerializeObject(total);
            Logger.LogDebug(totalContent);
            File.WriteAllText("valueChain.json", totalContent);
            ToExcel(total);
        }

        private List<ValueChain> GetValueChains(string sector)
        {
            var p = GetFileContent("ValueChainSteps").Replace("{sector}", sector);
            Logger.LogDebug($"Prompt: {p}");
            var activity = new ChatActivity(Logger, Endpoint, p, 0d);
            Logger.LogInformation($"Getting information for sector: {sector}");
            activity.ExecuteAsync().Wait();
            TotalTokens += activity.TokensUsed;
            var content = activity.Result.Values.First();
            Logger.LogDebug(content);
            var res = JsonConvert.DeserializeObject<List<ValueChain>>(content);
            for (int i = 0; i < res.Count; i++)
            {
                res[i].Step = (i + 1);
                res[i].Sector = sector;
            }
            return res;
        }

        public static void ToExcel(List<ValueChain> data)
        {
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, "valueChain.xlsx"));
            if (file.Exists) file.Delete();
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("ValueChain");
                var first = data.First();
                var row = 2;
                sheet.Cells[1, 1].Value = "Sector";
                sheet.Cells[1, 2].Value = "Step Number";
                sheet.Cells[1, 3].Value = "Value Chain Step";
                sheet.Cells[1, 4].Value = "Description";
                sheet.Cells[1, 5].Value = "Challenge";
                sheet.Cells[1, 6].Value = "Complete Name";
                foreach (var item in data)
                {
                    sheet.Cells[row, 1].Value = item.Sector;
                    sheet.Cells[row, 2].Value = item.Step;
                    sheet.Cells[row, 3].Value = item.Name;
                    sheet.Cells[row, 4].Value = item.Description;
                    sheet.Cells[row, 5].Value = item.Challenge;
                    sheet.Cells[row, 6].Value = $"{item?.Step?.ToString().PadLeft(2, '0')} - {item?.Name}";
                    row++;
                }
                // Save to file
                package.Save();
            }
        }

        private string GetFileContent(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            if (!name.EndsWith(".txt")) name += ".txt";
            return File.ReadAllText($"{Environment.CurrentDirectory}\\Prompts\\{name}");
        }
    }
}
