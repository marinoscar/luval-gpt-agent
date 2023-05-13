using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    public class ConsoleLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            var message = formatter(state, exception);

            WriteLine(logLevel, $"{DateTime.UtcNow:s} [{logLevel}] ({eventId}) {message}");

            if (exception != null)
            {
                Console.WriteLine(exception.ToString());
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Log everything by default
            return true;
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            // Not implemented
            return null;
        }

        private void WriteLine(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Trace:
                    WriteLineInfo(message);
                    break;
                case LogLevel.Debug:
                    WriteLineDebug(message);
                    break;
                case LogLevel.Information:
                    WriteLineInfo(message);
                    break;
                case LogLevel.Warning:
                    WriteLineWarning(message);
                    break;
                case LogLevel.Error:
                    WriteLineError(message);
                    break;
                case LogLevel.Critical:
                    WriteLineError(message);
                    break;
                case LogLevel.None:
                    WriteLineInfo(message);
                    break;
                default:
                    break;
            }
        }

        private void WriteLineDebug(string message)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }


        private void WriteLineInfo(string message)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }

        private void WriteLineWarning(string message)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }

        private void WriteLineError(string message)
        {
            var c = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }
    }
}
