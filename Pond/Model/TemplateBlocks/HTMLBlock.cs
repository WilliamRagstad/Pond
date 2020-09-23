using System;
using System.Collections.Generic;
using System.Text;

namespace Pond.Model.TemplateBlocks
{
    public class HTMLBlock : TemplateBlock
    {
        public string Text;

        public HTMLBlock(string text)
        {
            Text = text;
        }
    }
}
