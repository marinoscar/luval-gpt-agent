using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Luval.GPT.Agent.Core
{
    /// <summary>
    /// Provides extensions to the <see cref="IAgent"/> interface
    /// </summary>
    public static class AgentExtensions
    {
        /// <summary>
        /// Reads an environment variable
        /// </summary>
        /// <param name="agent">Agent</param>
        /// <param name="name">The name of the variable</param>
        /// <returns>The env variable value</returns>
        /// <exception cref="ArgumentNullException">If the env value does not exists</exception>
        public static string ReadEnvironmentVariable(this IAgent agent, string name)
        {
            var val = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
            if (string.IsNullOrWhiteSpace(val)) throw new ArgumentNullException($"No value available for Env Variable: {name}");
            return val;
        }

        /// <summary>
        /// Loads the input parameters from a file
        /// </summary>
        /// <param name="agent">Agent</param>
        public static void LoadInputParameters(this IAgent agent)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, agent.GetType().Name + ".json");
            LoadInputParameters(agent, fileName);
        }

        /// <summary>
        /// Loads the input parameters from a file
        /// </summary>
        /// <param name="agent">Agent</param>
        /// <param name="fileName">The file name</param>
        public static void LoadInputParameters(this IAgent agent, string fileName)
        {
            if (!File.Exists(fileName)) return;
            var content = File.ReadAllText(fileName);
            var items = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            if (items == null || !items.Any()) return;
            foreach (var item in items)
            {
                agent.InputParameters[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// Saves the InputParameters into a file
        /// </summary>
        /// <param name="agent">Agent</param>
        /// <param name="fileName">The file name</param>
        public static void SaveInputParametersToFile(this IAgent agent)
        {
            var fileName = Path.Combine(Environment.CurrentDirectory, agent.GetType().Name + ".json");
            SaveInputParametersToFile(agent, fileName);
        }

        /// <summary>
        /// Saves the InputParameters into a file
        /// </summary>
        /// <param name="agent">Agent</param>
        /// <param name="fileName">The file name</param>
        public static void SaveInputParametersToFile(this IAgent agent, string fileName)
        {
            var content = JsonConvert.SerializeObject(agent.InputParameters, Formatting.Indented);
            File.WriteAllText(fileName, content);
        }
    }
}
