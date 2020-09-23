using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;

namespace Pond.Model
{
    public class ChapterData
    {
        public HeaderBlock Title;
        public MarkdownBlock[] Elements;
    }
}
