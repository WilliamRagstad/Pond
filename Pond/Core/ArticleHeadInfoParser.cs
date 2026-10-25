using System;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Pond.Model;

namespace Pond.Core
{
    static class ArticleHeadInfoParser
    {
        // Metadata is only recognized in a leading JSON HTML comment.
        public static ArticleHeadInfo Parse(string article, ArticleHeadInfo fallback)
        {
            string content = File.ReadAllText(article);
            var match = Regex.Match(content, @"\A\s*<!--\s*(\{.*?\})\s*-->", RegexOptions.Singleline);
            if (!match.Success) return fallback;
            try
            {
                var info = JsonConvert.DeserializeObject<ArticleHeadInfo>(match.Groups[1].Value,
                    new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error });
                if (string.IsNullOrWhiteSpace(info?.Template)) throw new FormatException("Metadata requires Template.");
                return info;
            }
            catch (Exception error) { throw new FormatException($"Invalid article metadata: {error.Message}", error); }
        }
    }
}
