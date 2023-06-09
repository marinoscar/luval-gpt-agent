﻿using Luval.GPT.Agent.Core;
using Luval.GPT.Agent.Core.Data;
using Luval.GPT.Agent.Core.Model;
using Luval.Logging.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using MeetingNotesAgent = Luval.GPT.MeetingNotes.Agent.MeetingNotesAgent;

namespace Luval.GPT.Agent
{
    /// <summary>
    /// Application entry point
    /// </summary>
    class Program
    {

        /// <summary>
        /// Establish how to show the console window
        /// </summary>
        /// <param name="state">Values are available here: https://www.pinvoke.net/default.aspx/user32.showwindow</param>
        static void ShowWindowFunc(int state)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

            [DllImport("User32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);

            var handle = Process.GetCurrentProcess().MainWindowHandle;

            ShowWindow(handle, state);

        }


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

            if (arguments.ContainsSwitch("/winState") && !string.IsNullOrWhiteSpace(arguments["/winState"]))
            {
                var winState = arguments["/winState"];
                var winStateVal = 0;

                if(int.TryParse(winState, out winStateVal))
                {
                    ShowWindowFunc(winStateVal);
                }
            }

            var configFile = arguments["/config"];
            if(string.IsNullOrWhiteSpace(configFile)) throw new ArgumentNullException(nameof(configFile), $"Config file is required and it is missing, please provide one with switch /config");

            var services = new ServiceCollection();
            ConfigureServices(services);
            BuildAgents(configFile, services);


            var provider = services.BuildServiceProvider();
            var sw = Stopwatch.StartNew();


            WriteLine(ConsoleColor.Green, $"Starting Process");

            var logger = new CompositeLogger(new ILogger[] { new FileLogger(), new ColorConsoleLogger() });

            var agents = provider.GetServices<IAgent>();

            if (arguments.ContainsSwitch("/reset"))
            {
                logger.LogInformation("Resetting the data store");
                ResetData(arguments, provider);
                return;
            }

            var agentTasks = new List<Task>();
            foreach (var agent in agents)
            {
                var reg = provider.GetService<AgentRegistration>();
                LoadParametersFromService(agent, reg);
                agent.LoadInputParameters();
                agentTasks.Add(Task.Run(() => agent.ExecuteAsync()));
            }

            Task.WaitAll(agentTasks.ToArray());

            sw.Stop();
            var message = $"Process Completed in {sw.Elapsed}";
            WriteLine(ConsoleColor.Green, message);
            logger.LogInformation(message);

        }

        /// <summary>
        /// Loads the parameters from the agent registration
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="reg"></param>
        static void LoadParametersFromService(IAgent agent, AgentRegistration? reg)
        {
            if (reg != null)
            {
                var config = reg.Configurations.SingleOrDefault(i => i.TypeName != null && i.TypeName.ToLowerInvariant().Equals(agent.GetType().FullName.ToLowerInvariant()));
                if (config != null)
                {
                    foreach (var item in config.InputParameters)
                    {
                        agent.InputParameters[item.Key] = item.Value;
                    }
                }
            }
        }

        /// <summary>
        /// Reset the local database
        /// </summary>
        static void ResetData(ConsoleSwitches args, IServiceProvider provider)
        {
            if (!args.ContainsSwitch("/reset")) return;
            var repo = provider.GetService<IAgentRepository>();
            repo.ResetData();
        }

        /// <summary>
        /// Builds the agents based on the configuration file
        /// </summary>
        /// <param name="agentConfigurationFileName"></param>
        /// <param name="services"></param>
        static void BuildAgents(string agentConfigurationFileName, ServiceCollection services)
        {
            var registration = LoadAgents(agentConfigurationFileName);
            services.AddSingleton(registration);
            foreach (var item in registration.Configurations)
            {
                var ass = Assembly.Load(item.AssemblyName);
                var type = ass.GetType(item.TypeName);
                services.AddTransient(typeof(IAgent), type);
            }
        }

        /// <summary>
        /// Gets the configuration for the agents
        /// </summary>
        /// <param name="agentConfigurationFileName">The file that contains the configuration</param>
        /// <returns>An instance of <see cref="AgentRegistration"/> </returns>
        static AgentRegistration LoadAgents(string agentConfigurationFileName)
        {
            if (!File.Exists(agentConfigurationFileName)) throw new FileNotFoundException($"Configuration file for agent not found: {agentConfigurationFileName}");
            var result = JsonConvert.DeserializeObject<AgentRegistration>(File.ReadAllText(agentConfigurationFileName));
            return result;
        }

        /// <summary>
        /// Configure the dependencies
        /// </summary>
        static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger>(new CompositeLogger(new ILogger[] { new FileLogger(), new ColorConsoleLogger() }));
            services.AddTransient<IAgentRepository>((s) => { return new AgentRepository(); });
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