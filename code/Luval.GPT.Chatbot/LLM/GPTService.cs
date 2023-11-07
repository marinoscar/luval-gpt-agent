using Luval.Framework.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Chatbot.LLM
{
    public class GPTService : LuvalServiceBase<string>
    {
        private readonly GPTProvider _provider;

        public GPTService(GPTProvider gptProvider, ILogger logger) : this(gptProvider, logger, new ServiceConfiguration() { NumberOfRetries = 3, RetryIntervalInMs = 1000 })
        {

        }

        public GPTService(GPTProvider gptProvider, ILogger logger, ServiceConfiguration serviceConfiguration) : base(logger, "GPTService", serviceConfiguration)
        {
            _provider = gptProvider;
        }

        public GPTConfiguration? Configuration { get; set; }

        protected async override Task<ServiceResponse<string>> DoExecuteAsync()
        {
            if(Configuration == null) throw new ArgumentNullException(nameof(Configuration));

            var response = await _provider.RespondToMessageAsync(Configuration.ChatMessages, Configuration.IncomingMessage);
            return new ServiceResponse<string> { Message = "Success", Result = response, Status = ServiceStatus.Completed };

        }
    }
}
