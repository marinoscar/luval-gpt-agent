using Luval.GPT.Agent.Core;
using Luval.GPT.Agent.Core.Activity;
using Luval.GPT.Agent.Core.Data;
using Luval.GPT.MeetingNotes.Activities;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Microsoft.Extensions.Logging;
using System.Net;

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
            if (string.IsNullOrWhiteSpace(InputParameters["OpenAIKey"]))
                InputParameters["OpenAIKey"] = this.ReadEnvironmentVariable("OpenAIKey");
            if (string.IsNullOrWhiteSpace(InputParameters["SpeechKey"]))
                InputParameters["SpeechKey"] = this.ReadEnvironmentVariable("AzureAudioKey");
        }

        protected async override Task OnExecuteAsync()
        {
            Initialize();
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

                Result["ConvertedAudioFile"] = transcriber.Result["ConvertedAudio"];
                Result["ResultAudioFile"] = transcriber.Result["ResultFile"];
                Result["TranscriptFile"] = transcriber.Result["TranscriptFile"];


                var text = File.ReadAllText(transcriber.Result["TranscriptFile"]);
                var summarizer = new SummarizeActivity(Logger, create);
                var actionItems = new ActionItemsActivity(Logger, create);
                var summaryFile = await RunChunkActivities(new FileInfo(audioFile), summarizer, text, "summary.txt");
                var actionFile = await RunChunkActivities(new FileInfo(audioFile), actionItems, text, "actions.txt");

                Result["SummaryFile"] = summaryFile;
                Result["ActionItemFile"] = actionFile;
            }
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
            var fileName = audioFile.Name.Replace(audioFile.Extension, "") + "-" + suffix;
            var outputFile = Path.Combine(InputParameters["DestinationFolder"], fileName);
            chunkActivity.InputParameters["Text"] = text;
            await RunActivity(chunkActivity);
            File.WriteAllText(outputFile, chunkActivity.Result.Values.First());
            return outputFile;

        }
    }
}
