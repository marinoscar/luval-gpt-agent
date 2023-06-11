﻿using Luval.GPT.Agent.Core;
using Luval.GPT.Agent.Core.Activity;
using Luval.GPT.Agent.Core.Data;
using Luval.GPT.MeetingNotes.Activities;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json.Nodes;

namespace Luval.GPT.MeetingNotes.Agent
{
    public class MeetingNotesAgent : BaseAgent
    {

        public MeetingNotesAgent(ILogger logger) : this(logger, new AgentRepository())
        {

        }

        public MeetingNotesAgent(ILogger logger, IAgentRepository agentRepository) : base(logger, agentRepository)
        {
            Name = "Meeting Notes Agent";
            Description = "Creates meeting notes from an audio file";
        }

        private void Initialize()
        {
            if (!InputParameters.ContainsKey("OpenAIKey") || string.IsNullOrWhiteSpace(InputParameters["OpenAIKey"]))
                InputParameters["OpenAIKey"] = this.ReadEnvironmentVariable("OpenAIKey");

            if (!InputParameters.ContainsKey("SpeechKey") || string.IsNullOrWhiteSpace(InputParameters["SpeechKey"]))
                InputParameters["SpeechKey"] = this.ReadEnvironmentVariable("AzureAudioKey");
        }

        protected async override Task OnExecuteAsync()
        {
            Initialize();
            var results = new List<AnalyzerResult>();
            var speechConfig = new Speech2TextConfig() { Key = InputParameters["SpeechKey"] };
            var create = CreateEndpointFunc();

            var audioFiles = new FindAudioFilesActivity(Logger);
            audioFiles.InputParameters["WorkingDirectory"] = InputParameters["WorkingDirectory"];
            audioFiles.InputParameters["DestinationFolder"] = InputParameters["DestinationFolder"];
            await RunActivity(audioFiles);

            foreach (var audioFile in audioFiles.Result.Values)
            {
                var transcriber = new TranscribeAudioFileActivity(Logger, new AudioTranscriber(speechConfig, Logger), new AudioFormatConverter(audioFile, Logger));
                transcriber.InputParameters["WorkingDirectory"] = InputParameters["WorkingDirectory"];
                transcriber.InputParameters["DestinationFolder"] = InputParameters["DestinationFolder"];
                await RunActivity(transcriber);

                var text = File.ReadAllText(transcriber.Result["TranscriptFile"]);
                var summarizer = new SummarizeActivity(Logger, create);
                var actionItems = new ActionItemsActivity(Logger, create);
                var summaryFile = await RunChunkActivities(new FileInfo(audioFile), summarizer, text, "summary.txt");
                var actionFile = await RunChunkActivities(new FileInfo(audioFile), actionItems, text, "actions.txt");

                var summary = File.ReadAllText(summaryFile);
                var title = await GetTitle(create(), summary);

                results.Add(new AnalyzerResult()
                {
                    ActionItems = actionFile,
                    Summary = summaryFile,
                    Transcript = transcriber.Result["TranscriptFile"],
                    Subject = title,
                });

                //Delete temp files
                if(File.Exists(transcriber.Result["ConvertedAudio"]))
                    File.Delete(transcriber.Result["ConvertedAudio"]);
            }

            Result["Result"] = JsonConvert.SerializeObject(results);
        }

        private Func<ChatEndpoint> CreateEndpointFunc()
        {
            return () =>
            {
                ChatEndpoint ai;
                if (!string.IsNullOrWhiteSpace(InputParameters["UseOpenAIAzure"])
                    && InputParameters["UseOpenAIAzure"].ToLower().Equals("true"))
                {
                    var env = InputParameters["OpenAIAzureEnvironment"];
                    if (string.IsNullOrEmpty(env)) throw new ArgumentException("If the InputParameter UseOpenAIAzure is true then a value is required for InputParameter OpenAIAzureEnvironment");
                    ai = ChatEndpoint.CreateAzure(new ApiAuthentication(new NetworkCredential("", InputParameters["OpenAIKey"]).SecurePassword), env);
                }
                else
                    ai = ChatEndpoint.CreateOpenAI(new ApiAuthentication(new NetworkCredential("", InputParameters["OpenAIKey"]).SecurePassword));
                return ai;
            };
        }

        private async Task<string> RunChunkActivities(FileInfo audioFile, ChunkActivityBase chunkActivity, string text, string suffix)
        {
            var outputFile = GetNewFileName(audioFile, suffix);
            chunkActivity.InputParameters["Text"] = text;
            await RunActivity(chunkActivity);
            File.WriteAllText(outputFile, chunkActivity.Result.Values.First());
            return outputFile;

        }

        private string GetNewFileName(FileInfo file, string suffix)
        {
            var fileName = file.Name.Replace(file.Extension, "") + "-" + suffix;
            return  Path.Combine(InputParameters["DestinationFolder"], fileName);
        }

        private async Task<string> GetTitle(ChatEndpoint endpoint, string summary)
        {
            var p = @"
From the following summary of text please provide a title for the subject discussed
Here is the summary:
{Text}
";
            var a = new GetSummaryTitleActivity(Logger, endpoint, p)
            {
                InputParameters = new Dictionary<string, string> {
                    { "Text", summary }
                }
            };
            await RunActivity(a);
            return a.Result.Values.First();
        }
    }
}
