using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Pond.Model.TemplateBlocks;

namespace Pond.Model
{
    public class TemplateDocument
    {
        public TemplateBlock[] Elements;
        public static TemplateDocument ParseFile(string filepath)
        {
            try { return Parse(File.ReadAllText(filepath)); }
            catch (FormatException error) { throw new FormatException($"Template '{filepath}': {error.Message}", error); }
        }
        public static TemplateDocument Parse(string content)
        {
            int position = 0;
            FormatException Error(string message, int offset) => new FormatException($"{message} at offset {offset}.");
            TemplateBlock[] Read(string expected = null)
            {
                var blocks = new List<TemplateBlock>();
                while (position < content.Length)
                {
                    int text = content.IndexOf("{{", position, StringComparison.Ordinal);
                    int action = content.IndexOf("{%", position, StringComparison.Ordinal);
                    int start = text < 0 ? action : action < 0 ? text : Math.Min(text, action);
                    if (start < 0) { blocks.Add(new HTMLBlock(content.Substring(position))); position = content.Length; break; }
                    if (start > position) blocks.Add(new HTMLBlock(content.Substring(position, start - position)));
                    bool isText = start == text;
                    string delimiter = isText ? "}}" : "%}";
                    int end = content.IndexOf(delimiter, start + 2, StringComparison.Ordinal);
                    if (end < 0) throw Error($"Unterminated tag; expected {delimiter}", start);
                    string expression = content.Substring(start + 2, end - start - 2).Trim();
                    position = end + 2;
                    if (expression.Length == 0) throw Error("Empty template tag", start);
                    if (isText) { blocks.Add(new TextBlock(expression)); continue; }
                    if (expression == "end" || expression == "endif" || expression == "endfor")
                    {
                        if (expected == null || (expression != "end" && expression != "end" + expected))
                            throw Error($"Unexpected {expression}", start);
                        return blocks.ToArray();
                    }
                    string keyword = Regex.Split(expression, @"\s+")[0];
                    if (keyword == "for" && !Regex.IsMatch(expression, @"^for\s+[A-Za-z_]\w*\s+in\s+.+$", RegexOptions.Singleline))
                        throw Error("Expected for <variable> in <expression>", start);
                    if (keyword != "for" && keyword != "if") throw Error($"Unknown action '{keyword}'", start);
                    if (expression == "if") throw Error("Missing if expression", start);
                    blocks.Add(new ActionBlock(expression, Read(keyword)));
                }
                if (expected != null) throw Error($"Missing end{expected}", position);
                return blocks.ToArray();
            }
            return new TemplateDocument { Elements = Read() };
        }
    }
}
