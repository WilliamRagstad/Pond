using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;

namespace Pond.Extensions
{
    public static class MarkdownBlockExtensions
    {
        public static string ToHtml(this MarkdownBlock block, string indent = "")
        {
            if (block is ParagraphBlock paragraph) return $"<p>\n{indent + Settings.Indent + paragraph.Inlines.ToHtml(indent + Settings.Indent).TrimStart()}</p>";
            if (block is CodeBlock code) return $"<code class=\"language-{code.CodeLanguage}\">{code.Text}</code>";
            if (block is HeaderBlock header) return $"<h{header.HeaderLevel}>{header.Inlines.ToHtml(indent + Settings.Indent) + indent}</h{header.HeaderLevel}>";
            if (block is HorizontalRuleBlock) return $"<hr>";
            if (block is ListBlock list)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<li>");
                foreach (ListItemBlock item in list.Items)
                {
                    foreach (MarkdownBlock itemBlock in item.Blocks)
                    {
                        sb.AppendLine(indent + itemBlock.ToHtml(indent + Settings.Indent));
                    }
                }
                sb.AppendLine("</li>");
                return sb.ToString();
            }
            return block.ToString();
        }
    }
}
