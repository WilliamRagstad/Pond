using System;
using System.Collections.Generic;
using System.Text;

namespace Pond.Model.TemplateBlocks
{
    public class ActionBlock : TemplateBlock
    {
        public string ActionExpression;
        public TemplateBlock[] Elements;

        public ActionBlock(string actionExpression, TemplateBlock[] elements)
        {
            ActionExpression = actionExpression;
            Elements = elements;
        }
    }
}
