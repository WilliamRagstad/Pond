using System;
using System.Collections.Generic;
using System.Text;

namespace Pond.Model.TemplateBlocks
{
    public class TextBlock : TemplateBlock
    {
        public string TextExpression;

        public TextBlock(string textExpression)
        {
            TextExpression = textExpression;
        }
    }
}
