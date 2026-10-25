using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;

namespace Pond.Extensions
{
    public static class MarkdownBlockExtensions
    {
        public static string ToHtml(this MarkdownBlock block)
        {
            if (block is ParagraphBlock paragraph) return $"<p>{paragraph.Inlines.ToHtml().TrimStart()}</p>";
            if (block is CodeBlock code) return $"<pre><code class=\"language-{WebUtility.HtmlEncode(code.CodeLanguage)}\">{WebUtility.HtmlEncode(code.Text)}</code></pre>";
            if (block is HeaderBlock header) return $"<h{header.HeaderLevel}>{header.Inlines.ToHtml()}</h{header.HeaderLevel}>";
            if (block is HorizontalRuleBlock) return $"<hr>";
            if (block is ListBlock list)
            {
                StringBuilder sb = new StringBuilder();
                var tag = list.Style.ToString() == "Numbered" ? "ol" : "ul";
                sb.Append("<" + tag + ">\n");
                foreach (ListItemBlock item in list.Items)
                {
                    sb.Append("<li>");
                    foreach (MarkdownBlock itemBlock in item.Blocks)
                    {
                        sb.Append(itemBlock.ToHtml());
                    }
                    sb.Append("</li>\n");
                }
                sb.Append("</" + tag + ">\n");
                return sb.ToString();
            }
            return block.ToString();
        }
    }
}
