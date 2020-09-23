using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;

namespace Pond.Extensions
{
    public static class MarkdownInlineExtensions
    {
        public static string ToHtml(this MarkdownInline inline, string indent = "", bool keepComments = false)
        {
            if (inline is ImageInline image) return $"<img src=\"{image.Url}\" alt=\"{image.Tooltip}\" id=\"{image.ReferenceId}\"/>";
            if (inline is CodeInline code) return $"<code>{code.Text}</code>";
            if (inline is BoldTextInline bold) return $"<b>{bold.Inlines.ToHtml(indent + Settings.Indent).TrimStart().TrimEnd()}</b>";
            if (inline is ItalicTextInline italc) return $"<em>{italc.Inlines.ToHtml(indent + Settings.Indent).TrimStart().TrimEnd()}</em>";
            if (inline is StrikethroughTextInline strikethrough) return $"<strike>{strikethrough.Inlines.ToHtml(indent + Settings.Indent).TrimStart().TrimEnd()}</strike>";
            if (inline is HyperlinkInline hyperlink) return $"<a href=\"{hyperlink.Url}\" data-type=\"{hyperlink.LinkType}\">{hyperlink.Text}</a>";
            if (inline is MarkdownLinkInline link) return $"<a href=\"{link.Url}\">{link.Inlines.ToHtml()}</a>";
            if (inline is EmojiInline emoji) return $"<i>{emoji.Text}</i>";
            if (inline is TextRunInline text) return text.Text;
            if (inline is SuperscriptTextInline superscript) return $"<sup>{superscript.Inlines.ToHtml(indent + Settings.Indent).TrimStart().TrimEnd()}</sup>";
            if (inline is SubscriptTextInline subscript) return $"<sub>{subscript.Inlines.ToHtml(indent + Settings.Indent).TrimStart().TrimEnd()}</sub>";
            if (!keepComments && inline.Type == MarkdownInlineType.Comment) return "";
            return inline.ToString();
        }

        public static string ToHtml(this IList<MarkdownInline> children, string indent = "")
        {
            StringBuilder sb = new StringBuilder();
            foreach (MarkdownInline child in children) sb.AppendLine(indent + child.ToHtml(indent + Settings.Indent));
            return sb.ToString();
        }
    }
}
