using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using DynamicExpresso;
using Pond.Extensions;
using Pond.Model;
using Pond.Model.TemplateBlocks;

namespace Pond.Core
{
    public static class TemplateAssembler
    {
        public static string AssembleFile(ArticleData data, string templateFile)
        {
            try { return Assemble(data, TemplateDocument.ParseFile(templateFile)); }
            catch (Exception error) { throw new InvalidOperationException($"Template '{templateFile}': {error.Message}", error); }
        }
        // Kept for source compatibility; each evaluation now has its own scope.
        public static void InitializeInterpreter(ArticleData data) { }
        public static string Assemble(ArticleData data, TemplateDocument document) => Assemble(data, document.Elements);
        public static string Assemble(ArticleData data, TemplateBlock[] elements) => Assemble(data, elements, new Dictionary<string, object>());
        public static string Assemble(ArticleData data, TemplateBlock[] elements, Dictionary<string, object> locals)
        {
            object Evaluate(string expression)
            {
                var interpreter = new Interpreter(InterpreterOptions.Default);
                foreach (var field in typeof(ArticleData).GetFields()) interpreter.SetVariable(field.Name, field.GetValue(data), field.FieldType);
                interpreter.Reference(typeof(MarkdownBlockExtensions));
                foreach (var variable in locals) interpreter.SetVariable(variable.Key, variable.Value);
                try { return interpreter.Eval(expression); }
                catch (Exception error) { throw new InvalidOperationException($"Failed to evaluate '{expression}': {error.Message}", error); }
            }
            var result = new StringBuilder();
            foreach (var block in elements)
            {
                if (block is HTMLBlock html) result.Append(html.Text);
                else if (block is TextBlock text) result.Append(Evaluate(text.TextExpression));
                else if (block is ActionBlock action)
                {
                    var loop = Regex.Match(action.ActionExpression, @"^for\s+([A-Za-z_]\w*)\s+in\s+(.+)$", RegexOptions.Singleline);
                    if (loop.Success)
                    {
                        if (!(Evaluate(loop.Groups[2].Value) is IEnumerable values)) throw new InvalidOperationException($"For expression is not enumerable: {loop.Groups[2].Value}");
                        foreach (var value in values)
                        {
                            var scope = new Dictionary<string, object>(locals) { [loop.Groups[1].Value] = value };
                            result.Append(Assemble(data, action.Elements, scope));
                        }
                    }
                    else
                    {
                        string condition = action.ActionExpression.Substring(2).Trim();
                        if (!(Evaluate(condition) is bool enabled)) throw new InvalidOperationException($"If expression must be boolean: {condition}");
                        if (enabled) result.Append(Assemble(data, action.Elements, locals));
                    }
                }
            }
            return result.ToString();
        }
    }
}
