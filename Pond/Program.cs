using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ArgumentsUtil;
using Newtonsoft.Json;
using Pond.Core;
using Pond.Model;
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

        private static readonly bool Debug = false;
        static void Main(string[] args)
        {
            args = new [] {"build", @"C:\Users\willi\Desktop\pond test\config.json"};

            Arguments a = Arguments.Parse(args);

            if (a.ContainsKey("h") || a.ContainsKey("?") || args.Length == 0 || a.Keyless.Count == 0) At.ShowManual();
            else if (a.Keyless[0] == "init")
            {
                // Initialize new Pond project
                #region Initialize

                string cd = Directory.GetCurrentDirectory();
                string json = JsonConvert.SerializeObject(Config.Default, Formatting.Indented);
                string configPath = Path.Combine(cd, "config.json");
                Console.WriteLine($"Creating {configPath}...");
                File.WriteAllText(configPath, json);

                #region Folders
                    
                Directory.CreateDirectory(Path.Combine(cd, Config.Default.ArticlesPath));
                Directory.CreateDirectory(Path.Combine(cd, Config.Default.TemplatesPath));
                Directory.CreateDirectory(Path.Combine(cd, Config.Default.StylesPath));
                Directory.CreateDirectory(Path.Combine(cd, Config.Default.OutputPath));

                #endregion

                string buildPath = Path.Combine(cd, "build.bat");
                Console.WriteLine($"Creating {buildPath}...");
                File.WriteAllText(buildPath, $@"@echo off
Pond build ""{configPath}""
pause
");
                Console.WriteLine("Done. Project was successfully initialized!", ConsoleColor.Green);

                #endregion
            }
            else if (a.Keyless.Count >= 2 && a.Keyless[0] == "build")
            {
                // Run Pond
                #region Build

                string configDirectory = Path.GetDirectoryName(a.Keyless[1]);

                Console.WriteLine("Building project...", ConsoleColor.Yellow);
                Config config = LoadConfig(a);

                if (config != null)
                {
                    PondCompiler.Compile(configDirectory, config, Debug);
                
                    Console.WriteLine("Done. Website was successfully built!", ConsoleColor.Green);
                }
                else Console.WriteLine("Could not load config!", ConsoleColor.Red);

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

            return null;
        }
    }
}
