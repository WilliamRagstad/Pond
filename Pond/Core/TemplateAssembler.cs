using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using DynamicExpresso;
using DynamicExpresso.Exceptions;
using Pond.Extensions;
using Pond.Model;
using Pond.Model.TemplateBlocks;

using Console = EzConsole.EzConsole;

namespace Pond.Core
{
    public static class TemplateAssembler
    {
        /// <summary>
        /// Assemble an article with the given template
        /// </summary>
        /// <param name="data">The article data</param>
        /// <param name="templateFile">The filepath to the template</param>
        /// <returns>True if successful</returns>
        public static string AssembleFile(ArticleData data, string templateFile) =>
            Assemble(data, TemplateDocument.ParseFile(templateFile));

        public static string Assemble(ArticleData data, TemplateDocument document) => Assemble(data, document.Elements);

        public static string Assemble(ArticleData data, TemplateBlock[] elements) =>
            Assemble(data, elements, new Dictionary<string, object>());
        public static string Assemble(ArticleData data, TemplateBlock[] elements, Dictionary<string, object> localVariables)
        {
            StringBuilder sb = new StringBuilder();
            foreach (TemplateBlock block in elements)
            {
                if (block is HTMLBlock html) sb.Append(html.Text);
                if (block is TextBlock text) sb.Append(TextEvaluator(text, data, localVariables).Trim());
                if (block is ActionBlock action) sb.Append(ActionEvaluator(action, data, localVariables));
            }
            return sb.ToString();
        }

        private static Interpreter _interpreter;

        public static void InitializeInterpreter(ArticleData data)
        {
            _interpreter = new Interpreter(InterpreterOptions.Default);

            // Set global variables
            foreach (FieldInfo fieldInfo in data.GetType().GetFields())
            {
                _interpreter.SetVariable(fieldInfo.Name, fieldInfo.GetValue(data));
            }

            _interpreter.Reference(typeof(MarkdownBlockExtensions));
        }
        private static string TextEvaluator(TextBlock text, ArticleData data) =>
            TextEvaluator(text, data, new Dictionary<string, object>());
        private static string TextEvaluator(TextBlock text, ArticleData data, Dictionary<string, object> localVariables)
        {
            string code = text.TextExpression.Trim();

            // Set local variables
            foreach (KeyValuePair<string, object> localVariable in localVariables)
            {
                _interpreter.SetVariable(localVariable.Key, localVariable.Value);
            }

            try
            {
                object result = _interpreter.Eval<object>(code);
                return result == null ? "null" : result.ToString();
            }
            catch
            {
                try
                {
                    return _interpreter.Eval<string>(code + ".ToString()");
                }
                catch (ParseException pe)
                {
                    Console.WriteLine($"Action Error: Failed to evaluate '{code}'. Parse Exception: {pe.Message}", ConsoleColor.Red);
                    return Settings.EvaluateError;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Action Error: Failed to evaluate '{code}'. Unknown exception: {e.Message}", ConsoleColor.Red);
                    return Settings.EvaluateError;
                }
            }
        }

        private static string ActionEvaluator(ActionBlock action, ArticleData data) =>
            ActionEvaluator(action, data, new Dictionary<string, object>());
        private static string ActionEvaluator(ActionBlock action, ArticleData data, Dictionary<string, object> localVariables)
        {
            StringBuilder sb = new StringBuilder();
            string keyword = action.ActionExpression.TrimStart().Split(' ')[0];
            switch (keyword)
            {
                case "for":
                    IEnumerable collection = ActionForGetEnumerable(action.ActionExpression, data, localVariables);
                    if (collection != null)
                    {
                        string enumeratorVariable = ActionForGetEnumeratorVariable(action.ActionExpression);
                        foreach (var element in collection)
                        {
                            localVariables[enumeratorVariable] = element;
                            sb.Append(Assemble(data, action.Elements, localVariables));
                        }
                    }
                    break;
                case "if":
                    string expression = ActionIfGetExpression(action.ActionExpression);
                    if (_interpreter.Eval<bool>(expression))
                    {
                        sb.Append(Assemble(data, action.Elements, localVariables));
                    }
                    break;
                default:
                    Console.WriteLine($"Action '{keyword}' is not a valid keyword!", ConsoleColor.Red);
                    break;
            }

            return sb.ToString();
        }

        #region Action Functions

        #region For
        
        private static IEnumerable ActionForGetEnumerable(string actionExpression, ArticleData data, Dictionary<string, object> localVariables)
        {
            string collection = actionExpression.Split("in")[1].Trim();

            // Interpreter collectionInterpreter = new Interpreter();
            foreach (KeyValuePair<string, object> variable in localVariables)
            {
                _interpreter.SetVariable(variable.Key, variable.Value);
            }

            try
            {
                return _interpreter.Eval<IEnumerable>(collection);
            }
            catch { }

            Console.WriteLine($"Action Error: Could not find collection '{collection}'!", ConsoleColor.Red);
            return null;
        }

        private static string ActionForGetEnumeratorVariable(string actionExpression) => actionExpression.Split("for")[1].Split("in")[0].Trim();

        #endregion

        #region If


        private static string ActionIfGetExpression(string actionExpression) => actionExpression.Split("if")[1].Trim();

        #endregion

        #endregion
    }
}
