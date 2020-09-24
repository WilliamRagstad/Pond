using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pond.Model.TemplateBlocks;

namespace Pond.Model
{
    public class TemplateDocument
    {
        public TemplateBlock[] Elements;

        /// <summary>
        /// Parses the specified file assuming it exists
        /// </summary>
        /// <param name="filepath">file</param>
        /// <returns>Template document</returns>
        public static TemplateDocument ParseFile(string filepath) => Parse(File.ReadAllText(filepath));
        public static TemplateDocument Parse(string content)
        {
            List<TemplateBlock> elements = new List<TemplateBlock>();
            string cache = string.Empty;
            int i = 0;

            #region Helper functions

            void AddHTMLCache()
            {
                if (!string.IsNullOrEmpty(cache))
                {
                    elements.Add(new HTMLBlock(cache));
                }
                cache = string.Empty; // Clear cache
            }

            #endregion

            #region Parsers

            void ParseText()
            {
                string expression = string.Empty;
                for (; i < content.Length - 1; i++)
                {
                    char cc = content[i];
                    char nc = content[i + 1];

                    if (cc == '}' && nc == '}') break;
                    expression += cc;
                }
                elements.Add(new TextBlock(expression));
            }

            void ParseAction()
            {
                string expression = string.Empty;
                // Expression
                for (; i < content.Length - 1; i++)
                {
                    char cc = content[i];
                    char nc = content[i + 1];

                    if (cc == '%' && nc == '}') break;
                    expression += cc;
                }

                i += 2;

                string actionType = expression.TrimStart().Split(' ')[0].ToLower();
                string actionContent = string.Empty;
                int actionDepth = 1;
                // Content
                for (; i < content.Length - 1; i++)
                {
                    char cc = content[i];
                    char nc = content[i + 1];

                    if (cc == '{' && nc == '%')
                    {
                        i += 2;
                        string buff = "{%";
                        for (; i < content.Length - 1; i++)
                        {
                            buff += content[i];
                            if (content[i] != ' ') break;
                        }
                        char e = content[i];
                        char n = content[i + 1];
                        char d = content[i + 2];
                        if (char.ToLower(e) == 'e' && char.ToLower(n) == 'n' && char.ToLower(d) == 'd') actionDepth--;
                        else actionDepth++;
                        if (actionDepth == 0) break;
                        actionContent += buff;
                        continue;
                    }
                    actionContent += cc;
                }

                // Iterate till the end of the last ' %}'
                for (; i < content.Length; i++)
                {
                    char pc = content[i - 1];
                    char cc = content[i];
                    if (pc == '%' && cc == '}') break;
                }

                TemplateDocument actionInnerDocument = Parse(actionContent);
                elements.Add(new ActionBlock(expression, actionInnerDocument.Elements));
            }

            #endregion

            for (; i < content.Length; i++)
            {
                char cc = content[i];
                if (i < content.Length - 1)
                {
                    char nc = content[i + 1];

                    if (cc == '{' && (nc == '{' || nc == '%'))
                    {
                        cache = TrimEnd(cache);
                        AddHTMLCache();
                        i += 2; // Skip opening parenthesis
                        if      (nc == '{') ParseText();
                        else if (nc == '%') ParseAction();
                        i++; // Skip closing parenthesis
                        if ("\r\n".Contains(content[i]))
                        {
                            cache = TrimEnd(cache);
                            i++;
                        }
                        continue;
                    }
                }

                cache += cc;
            }
            // Everything that's still in the cache is regarded as HTML.
            AddHTMLCache();
            return new TemplateDocument { Elements = elements.ToArray() };
        }

        #region Helper Functions

        private static string TrimEnd(string source)
        {
            int j = source.Length;
            bool addOneSpace = false;
            while (j > 0 && "\r\n\t ".Contains(source[j - 1]))
            {
                if (source[j - 1] == ' ') addOneSpace = true;
                j--;
            }
            return source.Substring(0, j) + (addOneSpace ? " " : "");
        }

        private static string TrimStart(string source)
        {
            int j = 0;
            bool addOneSpace = false;
            while (j < source.Length && "\r\n\t ".Contains(source[j]))
            {
                if (source[j] == ' ') addOneSpace = true;
                j++;
            }
            return  (addOneSpace ? " " : "") + source.Substring(j, source.Length - j);
        }

        #endregion
    }
}
