using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
            // args = "init /c file.json".Split(' ');

            Arguments a = Arguments.Parse(args);

            if (a.ContainsKey("h") || a.ContainsKey("?") || args.Length == 0 || a.Keyless.Count == 0) At.ShowManual();
            else if (a.Keyless[0] == "init")
            {
                // Initialize new Pond project
                #region Initialize

                string cd = Directory.GetCurrentDirectory();
                if (Confirm($"Are you sure you want to create a new project in '{cd}'? "))
                {
                    bool useConf = Confirm("Do you need a custom config file? ");
                    string configPath = null;
                    if (useConf)
                    {
                        string json = JsonConvert.SerializeObject(Config.Default, Formatting.Indented);
                        configPath = Path.Combine(cd, "config.json");
                        Console.WriteLine($"Creating {configPath}...");
                        File.WriteAllText(configPath, json);
                    }

                    string buildPath = Path.Combine(cd, "build.bat");
                    Console.WriteLine($"Creating {buildPath}...");
                    File.WriteAllText(buildPath, $@"@echo off
Pond build{( useConf ? $" \"{configPath}\"" : "" )}
pause
");
                    Console.WriteLine("Done! Project has successfully been initialize.", ConsoleColor.Green);
                }
                else Console.WriteLine("Project canceled.");

                #endregion
            }
            else if (a.Keyless[0] == "build")
            {
                // Run Pond
                #region Build

                Console.WriteLine("Building project...", ConsoleColor.Yellow);
                Config config = LoadConfig(a);

                Console.WriteLine("Compiling...", ConsoleColor.Yellow);
                PondCompiler.Compile(config);
                
                Console.WriteLine("Done! Website has successfully been built.", ConsoleColor.Green);

                #endregion
            }
            else Console.WriteLine("Unknown command, please use /? or /h for more help.", ConsoleColor.Red);
        }

        static bool Prompt(string message, bool ignoreCase, params string[] acceptedInput)
        {
            Console.Write(message);
            string ans = Console.ReadLine();
            if (ignoreCase) ans = ans.ToLower();
            for (int i = 0; i < acceptedInput.Length; i++)
            {
                if (acceptedInput[i].Equals(ans)) return true;
            }

            return false;
        }
        static bool Confirm(string message) => Prompt(message, true, "yes", "y", "yep", "yup", "ok");

        static Config LoadConfig(Arguments a)
        {
            if (a.Keyless.Count > 1)
            {
                string configFile = a.Keyless[1];
                Console.WriteLine("Loading config '" + configFile + "'...", ConsoleColor.Yellow);
                if (!File.Exists(configFile)) Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
                else if (Path.GetExtension(configFile).ToLower() != ".json") Console.WriteLine("Config file did not exist! Please enter a valid filepath.", ConsoleColor.Red);
                else return JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
            }
            Console.WriteLine("Loading default config...", ConsoleColor.Yellow);
            return Config.Default;
        }
    }
}
