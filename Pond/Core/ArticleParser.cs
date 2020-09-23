using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;
using Pond.Extensions;
using Pond.Model;
using Console = EzConsole.EzConsole;

namespace Pond.Core
{
    static class ArticleParser
    {
        public static ArticleData Parse(string article, ArticleHeadInfo articleHeadInfo, bool debug = false)
        {
            string content = File.ReadAllText(article);
            ArticleData ad = new ArticleData(articleHeadInfo);

            MarkdownDocument document = new MarkdownDocument();
            document.Parse(content);
            
            List<MarkdownBlock> elements = new List<MarkdownBlock>();
            List<ChapterData> chapters = new List<ChapterData>();
            ChapterData chapter = new ChapterData();
            List<MarkdownBlock> chapterElements = new List<MarkdownBlock>();

            void AddChapter()
            {
                if (chapterElements.Count > 0)
                {
                    chapter.Elements = chapterElements.ToArray();
                    chapters.Add(chapter);
                }
            }
            void NewChapter(HeaderBlock header)
            {
                // Add old
                AddChapter();
                // New chapter
                chapter = new ChapterData {Title = header};
                chapterElements = new List<MarkdownBlock>();
            }

            for (int i = 0; i < document.Blocks.Count; i++)
            {
                MarkdownBlock block = document.Blocks[i];
                if (debug) Console.Write($"{block.Type}: ", ConsoleColor.Cyan);
                
                if (!IsCommentParagraph(block)) elements.Add(block);

                if (block is HeaderBlock header)
                {
                    if (header.HeaderLevel <= 2)
                    {
                        if (header.HeaderLevel == 1 && ad.TITLE == null) ad.TITLE = header;
                        NewChapter(header);
                        continue;
                    }
                }

                if (!IsCommentParagraph(block)) chapterElements.Add(block);

                if (block is ParagraphBlock paragraph)
                {
                    if (debug) Console.WriteLine();
                    for (int j = 0; j < paragraph.Inlines.Count; j++)
                    {
                        MarkdownInline inline = paragraph.Inlines[j];
                        if (debug) Console.Write($"    {inline.Type}: ", ConsoleColor.DarkCyan);
                        if (debug) Console.WriteLine(inline.ToString());
                        if (inline.Type != MarkdownInlineType.Comment) { }
                    }
                    if (debug) Console.WriteLine("\n" + block.ToHtml());
                }
                else
                {
                    if (debug) Console.WriteLine(block + "\n" + block.ToHtml());
                }
            }

            AddChapter();
            ad.CHAPTERS = chapters.ToArray();
            ad.ELEMENTS = elements.ToArray();

            return ad;
        }

        private static bool IsCommentParagraph(MarkdownBlock block)
        {
            return block is ParagraphBlock p &&
                   p.Inlines.Count == 1 &&
                   p.Inlines[0].Type == MarkdownInlineType.Comment;
        }
    }
}
