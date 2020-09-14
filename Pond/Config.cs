using System;
using System.Collections.Generic;
using System.Text;

namespace Pond
{
    public class Config
    {
        public static Config Default = new Config("articles");

        public string ContentRoot;

        public Config(string contentRoot)
        {
            ContentRoot = contentRoot;
        }
    }
}
