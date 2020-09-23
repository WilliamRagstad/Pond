using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Pond.Model;
using Console = EzConsole.EzConsole;

namespace Pond.Core
{
    static class ArticleHeadInfoParser
    {
        public static ArticleHeadInfo Parse(string article, ArticleHeadInfo fallback)
        {
            string content = File.ReadAllText(article);
            Match info = Regex.Match(content, @"{[^}]+}");
            if (info.Success)
            {
                try
                {
                    return JsonConvert.DeserializeObject<ArticleHeadInfo>(info.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error while parsing '{article}':", ConsoleColor.Red);
                    Console.WriteLine(e.Message, ConsoleColor.Red);
                    // ignored, return fallback
                }
            }
            Console.WriteLine($"Using DefaultArticleInfo for article '{article}'", ConsoleColor.Yellow);
            return fallback;
        }
    }
}
