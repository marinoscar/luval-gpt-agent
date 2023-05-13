using Luval.GPT.Agent.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent
{
    public class CustomLogger : ILogger
    {

        private static ConsoleLogger ConsoleLogger = new ConsoleLogger();
        private static FileInfo fileInfo;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            ConsoleLogger.Log(logLevel, eventId, state, exception, formatter);

            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);
            Append($"{DateTime.UtcNow:s} [{logLevel}] ({eventId}) {message}");

            if (exception != null)
            {
                Append(exception.ToString());
            }
        }

        private static void Append(string message)
        {
            if (fileInfo == null)
            {
                fileInfo = new FileInfo("log.txt");
                if(fileInfo.Exists) fileInfo.Delete();
            }
            using (var s = File.AppendText(fileInfo.FullName))
            {
                s.WriteLine(message);
            }
        }
    }
}
