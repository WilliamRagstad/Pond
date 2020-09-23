using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pond.Extensions;
using Pond.Model;
using Pond.Model.TemplateBlocks;

namespace Pond.Core
{
    public static class TemplateAssembler
    {
        /// <summary>
        /// Assemble an article with the given template
        /// </summary>
        /// <param name="data">The article data</param>
        /// <param name="template">The filepath to the template</param>
        /// <returns>True if successful</returns>
        public static string Assemble(ArticleData data, string template)
        {
            StringBuilder sb = new StringBuilder();
            TemplateDocument document = TemplateDocument.ParseFile(template);
            foreach (TemplateBlock block in document.Elements)
            {
                if (block is HTMLBlock html) sb.Append(html.Text);
                if (block is TextBlock text) sb.Append(TextEvaluator(text, data).Trim());
                if (block is ActionBlock action) sb.Append(ActionEvaluator(action, data));
            }
            return sb.ToString();
        }

        private static string TextEvaluator(TextBlock text, ArticleData data)
        {
            string r = text.TextExpression;
            r = r.Replace("TITLE", data.TITLE.ToString());
            return r;
        }

        private static string ActionEvaluator(ActionBlock action, ArticleData data)
        {
            return "<!-- Action -->";
        }
    }
}
