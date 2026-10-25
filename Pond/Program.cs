using System;
using System.IO;
using Newtonsoft.Json;
using Pond.Core;
using Pond.Model;

namespace Pond
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length == 0 || (args.Length == 1 && (args[0] == "--help" || args[0] == "/h" || args[0] == "/?")))
                {
                    Console.WriteLine("Usage: Pond init | Pond build <config.json> | Pond build /c <config.json>");
                    return 0;
                }
                if (args.Length == 1 && args[0] == "init")
                {
                    using (var file = new StreamWriter(new FileStream("config.json", FileMode.CreateNew)))
                        file.Write(JsonConvert.SerializeObject(Config.Default, Formatting.Indented));
                    foreach (var path in new[] { "articles", "templates", "style", "site" }) Directory.CreateDirectory(path);
                    Console.WriteLine("Created config.json and project directories. Add Markdown and a template before building.");
                    return 0;
                }
                string configFile = args.Length == 2 && args[0] == "build" ? args[1] :
                    args.Length == 3 && args[0] == "build" && args[1] == "/c" ? args[2] :
                    throw new ArgumentException("Usage: Pond build <config.json> (or build /c <config.json>)");
                configFile = Path.GetFullPath(configFile);
                var config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile),
                    new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                PondCompiler.Compile(Path.GetDirectoryName(configFile), config);
                Console.WriteLine("Website built successfully.");
                return 0;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine("Pond: " + error.Message);
                return 1;
            }
        }
    }
}
