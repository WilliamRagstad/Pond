using System;
using System.Collections.Generic;
using System.Text;

namespace Pond
{
    public class Config
    {
        public static Config Default = new Config("articles", "site");

        public string ContentRoot, OutputPath;

        public Config(string contentRoot, string outputPath)
        {
            ContentRoot = contentRoot;
            OutputPath = outputPath;
        }
    }
}
