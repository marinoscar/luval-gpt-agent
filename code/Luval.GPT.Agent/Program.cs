﻿using Luval.FinanceResearch.Activities;
using Luval.GPT.Agent.Core;
using Luval.Logging.Providers;
using Luval.MN.Core.Activities;
using Luval.MN.Core.Agent;
using Luval.OpenAI;
using Luval.OpenAI.Chat;
using Luval.OpenAI.Completion;
using Luval.OpenAI.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Luval.GPT.Agent
{
    /// <summary>
    /// Application entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main entry point to the application
        /// </summary>
        /// <param name="args">Arguments</param>
        static void Main(string[] args)
        {
            /// Provides a way to parse the arguments <see cref="https://gist.github.com/marinoscar/d84265533b242a8a5e7eb74cdd50b7e5"/>
            var arguments = new ConsoleSwitches(args);
            RunAction(() =>
            {
                DoAction(arguments);

            }, false);
        }

        /// <summary>
        /// Executes an action on the application
        /// </summary>
        /// <param name="arguments"></param>
        static void DoAction(ConsoleSwitches arguments)
        {
            var sw = Stopwatch.StartNew();

            WriteLine(ConsoleColor.Green, $"Starting Process");
            var logger = new CompositeLogger(new ILogger[] { new FileLogger(), new ColorConsoleLogger() });
            var openAIKey = ReadEnvVariable("OpenAIKey");
            var speechKey = ReadEnvVariable("AzureAudioKey");

            var agent = new MeetingNotesAgent(logger);
            agent.InputParameters["OpenAIKey"] = openAIKey;
            agent.InputParameters["SpeechKey"] = speechKey;
            agent.InputParameters["WorkingDirectory"] = @"C:\Users\CH489GT\OneDrive - EY\Work\EY\Meeting Notes";
            agent.InputParameters["DestinationFolder"] = @"C:\Users\CH489GT\OneDrive - EY\Work\EY\Meeting Notes\Completed";
            agent.ExecuteAsync().Wait();

            sw.Stop();
            var message = $"Process Completed in {sw.Elapsed}";
            WriteLine(ConsoleColor.Green, message);
            logger.LogInformation(message);

        }

        /// <summary>
        /// Runs the action and handles exceptions
        /// </summary>
        /// <param name="action">The action to execute</param>
        public static void RunAction(Action action, bool waitForKey = false)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                WriteLineError(exception.ToString());
            }
            finally
            {
                if (waitForKey)
                {
                    WriteLineInfo("Press any key to end");
                    Console.ReadKey();
                }
            }
        }

        private static string ReadEnvVariable(string name)
        {
            var val = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (string.IsNullOrWhiteSpace(val)) throw new ArgumentNullException($"No value available for Env Variable: {name}");
            return val;
        }

        #region Console Methods

        /// <summary>
        /// Writes an message to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void Write(ConsoleColor color, string format, params object[] arg)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(format, arg);
            Console.ForegroundColor = current;
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void WriteLine(ConsoleColor color, string format, params object[] arg)
        {
            WriteLine(color, string.Format(format, arg));
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void WriteLine(string format, params object[] arg)
        {
            WriteLine(Console.ForegroundColor, string.Format(format, arg));
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="message">The string to format</param>
        public static void WriteLine(ConsoleColor color, string message)
        {
            var current = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = current;
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="message">The string to format</param>
        public static void WriteLine(string message)
        {
            WriteLine(Console.ForegroundColor, message);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void WriteLineInfo(string format, params object[] arg)
        {
            WriteLine(format, arg);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="message">The string to format</param>
        public static void WriteLineInfo(string message)
        {
            WriteLine(message);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void WriteLineWarning(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.Yellow, format, arg);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="message">The string to format</param>
        public static void WriteLineWarning(string message)
        {
            WriteLine(ConsoleColor.Yellow, message);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="format">The string to format</param>
        /// <param name="arg">The arguments to format the string</param>
        public static void WriteLineError(string format, params object[] arg)
        {
            WriteLine(ConsoleColor.Red, format, arg);
        }

        /// <summary>
        /// Writes a new line to the console
        /// </summary>
        /// <param name="color">The forground color of the message</param>
        /// <param name="message">The string to format</param>
        public static void WriteLineError(string message)
        {
            WriteLine(ConsoleColor.Red, message);
        }

        #endregion
    }
}