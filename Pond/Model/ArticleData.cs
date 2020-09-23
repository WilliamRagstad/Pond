using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;

namespace Pond.Model
{
    public class ArticleData
    {
        public ArticleHeadInfo HeadInfo { get; }
        public HeaderBlock TITLE;
        public ChapterData[] CHAPTERS;
        public MarkdownBlock[] ELEMENTS;

        public ArticleData(ArticleHeadInfo headInfo)
        {
            HeadInfo = headInfo;
        }
    }
}
