using System;
using System.IO;
using System.Linq;
using System.Text;
using Pond.Model;

namespace Pond.Core
{
    public static class PondCompiler
    {
        public static void Compile(string projectDirectory, Config config, bool debug = false)
        {
            if (config == null) throw new ArgumentException("Configuration must be a JSON object.");
            string Resolve(string name, string value)
            {
                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"Configuration requires {name}.");
                return Path.GetFullPath(Path.Combine(projectDirectory, value));
            }
            string articles = Resolve(nameof(config.ArticlesPath), config.ArticlesPath);
            string templates = Resolve(nameof(config.TemplatesPath), config.TemplatesPath);
            string styles = Resolve(nameof(config.StylesPath), config.StylesPath);
            string output = Resolve(nameof(config.OutputPath), config.OutputPath);
            if (string.IsNullOrWhiteSpace(config.DefaultArticleInfo?.Template))
                throw new ArgumentException("Configuration requires DefaultArticleInfo.Template.");
            foreach (var input in new[] { articles, templates, styles })
            {
                if (!Directory.Exists(input)) throw new DirectoryNotFoundException($"Input directory not found: {input}");
                var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                if (output.Equals(input, comparison) || output.StartsWith(input + Path.DirectorySeparatorChar, comparison) || input.StartsWith(output + Path.DirectorySeparatorChar, comparison))
                    throw new ArgumentException("OutputPath must not overlap input directories.");
            }
            foreach (var article in Directory.GetFiles(articles, "*", SearchOption.AllDirectories)
                .Where(p => Path.GetExtension(p).Equals(".md", StringComparison.OrdinalIgnoreCase)).OrderBy(p => p, StringComparer.Ordinal))
            {
                try
                {
                    var info = ArticleHeadInfoParser.Parse(article, config.DefaultArticleInfo);
                    string template = Path.Combine(templates, info.Template);
                    if (!File.Exists(template)) template = Path.Combine(templates, info.TemplateFileName());
                    if (!File.Exists(template)) throw new FileNotFoundException($"Template not found: {template}");
                    var data = ArticleParser.Parse(article, info, debug);
                    string result = TemplateAssembler.AssembleFile(data, template);
                    string destination = Path.Combine(output, Path.ChangeExtension(Path.GetRelativePath(articles, article), ".html"));
                    Directory.CreateDirectory(Path.GetDirectoryName(destination));
                    File.WriteAllText(destination, result.Replace("\r\n", "\n"), new UTF8Encoding(false));
                }
                catch (Exception error) { throw new InvalidOperationException($"Article '{article}': {error.Message}", error); }
            }
            foreach (var source in Directory.GetFiles(styles, "*", SearchOption.AllDirectories).OrderBy(p => p, StringComparer.Ordinal))
            {
                string destination = Path.Combine(output, "style", Path.GetRelativePath(styles, source));
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(source, destination, true);
            }
        }
    }
}
