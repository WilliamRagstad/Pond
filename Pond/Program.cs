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
        static void Main(string[] args)
        {
            args = "/c file.json".Split(' ');

            Arguments a = Arguments.Parse(args);

            if (args.Length == 0 || a.ContainsKey("h") || a.ContainsKey("?"))
            {
                ArgumentsTemplate at = new ArgumentsTemplate(
                    new List<ArgumentOption>
                    {
                        new ArgumentOption("c", "Configuration file", new List<ArgumentParameter>
                        {
                            new ArgumentParameter("file", typeof(string), "path to config file", true)
                        })
                    }
                );
                at.ShowManual();
                return;
            }

            Config config = LoadConfig(a);



        }

        static Config LoadConfig(Arguments a)
        {
            if (a.ContainsKey("c"))
            {
                string configFile = a["c"][0];
                if (string.IsNullOrEmpty(configFile)) Console.WriteLine("No config file was provided! Please enter a valid filepath.", ConsoleColor.Red);
                else if (!File.Exists(configFile)) Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
                else if (Path.GetExtension(configFile).ToLower() != ".json") Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
                else return JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            }
            return Config.Default;
        }
    }
}
