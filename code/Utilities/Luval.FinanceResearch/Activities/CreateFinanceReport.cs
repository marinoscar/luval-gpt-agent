using Luval.FinanceResearch.Inputs;
using Luval.GPT.Agent.Core;
using Luval.GPT.Agent.Core.Activity;
using Luval.OpenAI.Chat;
using Luval.OpenAI.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.FinanceResearch.Activities
{
    public class CreateFinanceReport
    {
        public ILogger Logger { get; set; }
        public Func<ChatEndpoint> Chatendpoint { get; set; }
        public CreateFinanceReport(ILogger logger, Func<ChatEndpoint> chatEndpoint)
        {
            Logger = logger;
            Chatendpoint = chatEndpoint;
        }

        public async Task<List<Usecase>> ExecuteAsync()
        {
            Logger.LogInformation($"Starting to create the report");
            var result = new List<Usecase>();
            foreach (var process in Prompts.ProcessAreas)
            {
                var subProcesses = await GetSubProcess(process);
                foreach (var subProcess in subProcesses)
                {
                    var kpis = await GetKpi(process, subProcess);
                    foreach (var kp in kpis)
                    {
                        foreach (var capability in Prompts.Capabilities)
                        {
                            var usecases = await GetUseCases(process, capability, subProcess, kp);
                            result.AddRange(usecases);
                        }
                    }
                }
            }
            Logger.LogInformation($"Report completed");
            var json = JsonConvert.SerializeObject(result);
            File.WriteAllText("Finance.json", json);
            ToExcel(result);
            return result;
        }

        public static void ToExcel(List<Usecase> data)
        {
            var file = new FileInfo(Path.Combine(Environment.CurrentDirectory, "Finance.xlsx"));
            if (file.Exists) file.Delete();
            using (var package = new ExcelPackage(file))
            {
                var sheet = package.Workbook.Worksheets.Add("Data");
                var first = data.First();
                var row = 2;
                sheet.Cells[1, 1].Value = "Process";
                sheet.Cells[1, 2].Value = "Sub Process";
                sheet.Cells[1, 3].Value = "Sub Process Description";
                sheet.Cells[1, 4].Value = "KPI";
                sheet.Cells[1, 5].Value = "KPI Description";
                sheet.Cells[1, 6].Value = "Capability";
                sheet.Cells[1, 7].Value = "Title";
                sheet.Cells[1, 8].Value = "Description";
                sheet.Cells[1, 9].Value = "Implementation";
                sheet.Cells[1, 10].Value = "Endpoint";
                sheet.Cells[1, 11].Value = "Custom Model";

                foreach (var item in data)
                {
                    sheet.Cells[row, 1].Value = item.FinanceArea;
                    sheet.Cells[row, 2].Value = item.SubProcess;
                    sheet.Cells[row, 3].Value = item.SubProcessDescription;
                    sheet.Cells[row, 4].Value = item.KeyPerformanceIndicator;
                    sheet.Cells[row, 5].Value = item.KPIDescription;
                    sheet.Cells[row, 6].Value = item.Capability;
                    sheet.Cells[row, 7].Value = item.Title;
                    sheet.Cells[row, 8].Value = item.Description;
                    sheet.Cells[row, 9].Value = item.Implementation;
                    sheet.Cells[row, 10].Value = item.Endpoint;
                    sheet.Cells[row, 11].Value = item.CustomModel;
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

        private async Task<List<FinanceInfo>> GetSubProcess(string process)
        {
            var result = new List<FinanceInfo>();
            var activity = CreateActivity("Getting finance sub processes", Prompts.SubProcesses,
                new Dictionary<string, string>() {
                    { "area", process }
                });
            await activity.ExecuteAsync();
            result = activity.CastResult<List<FinanceInfo>>();
            return result;
        }

        private async Task<List<FinanceInfo>> GetKpi(string process, FinanceInfo subProcess)
        {
            var result = new List<FinanceInfo>();
            var activity = CreateActivity("Getting KPI", Prompts.SubProcessKPI,
                new Dictionary<string, string>() {
                    { "area", process },
                    { "name", subProcess.Name },
                    { "description", subProcess.Description }
                });
            await activity.ExecuteAsync();
            result = activity.CastResult<List<FinanceInfo>>();
            return result;
        }

        private async Task<List<Usecase>> GetUseCases(string process, string capability, FinanceInfo subProcess, FinanceInfo kpi)
        {
            var result = new List<Usecase>();
            var activity = CreateActivity("Getting Use Cases", Prompts.SubProcessKPIUseCase,
                new Dictionary<string, string>() {
                    { "area", process },
                    { "name", subProcess.Name },
                    { "description", subProcess.Description },
                    { "kpi", subProcess.Name },
                    { "kpi-description", subProcess.Description },
                    { "capabilities", capability }
                });
            await activity.ExecuteAsync();
            result = activity.CastResult<List<Usecase>>();
            result.ForEach(i => {
                i.FinanceArea = process;
                i.SubProcess = subProcess.Name;
                i.SubProcessDescription = subProcess.Description;
                i.KeyPerformanceIndicator = kpi.Name;
                i.KPIDescription = kpi.Description;
            });
            return result;
        }

        private ChatActivity CreateActivity(string name, string prompt, Dictionary<string, string> parameters)
        {
            return new ChatActivity(Logger, Chatendpoint(), prompt, 0d) { Name = name, InputParameters = parameters };
        }

    }
}
