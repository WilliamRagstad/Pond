using System;
using System.Collections.Generic;
using System.IO;
using ArgumentsUtil;
using Newtonsoft.Json;
using Console = EzConsole.EzConsole;

namespace Pond
{
    class Program
    {
        static readonly ArgumentsTemplate At = new ArgumentsTemplate(
            new List<ArgumentOption>
            {
                new ArgumentOption("c", "Configuration file", new List<ArgumentParameter>
                {
                    new ArgumentParameter("file", typeof(string), "path to config file")
                })
            }, false,
            new List<ArgumentCommand>
            {
                new ArgumentCommand("init", "Initialize a new Pond project")
            }
        );
        static void Main(string[] args)
        {
            args = "init /c file.json".Split(' ');

            Arguments a = Arguments.Parse(args);

            if (a.ContainsKey("h") || a.ContainsKey("?"))At.ShowManual();
            else if (a.Keyless[0] == "init")
            {
                // Initialize new Pond project
                Console.WriteLine("Project initialization is not supported in this experimental version! Please wait for official release.", ConsoleColor.Yellow);

            }
            else if (a.ContainsKey("c"))
            {
                Config config = LoadConfig(a);
                Console.WriteLine("This feature is not supported in this experimental version! Please wait for official release.", ConsoleColor.Yellow);

                // Run Pond
            }
            else At.ShowManual();
        }

        static Config LoadConfig(Arguments a)
        {
            string configFile = a["c"][0];
            if (string.IsNullOrEmpty(configFile)) Console.WriteLine("No config file was provided! Please enter a valid filepath.", ConsoleColor.Red);
            else if (!File.Exists(configFile)) Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
            else if (Path.GetExtension(configFile).ToLower() != ".json") Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
            else return JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            return Config.Default;
        }
    }
}
