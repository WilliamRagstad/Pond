using System;
using System.IO;
using Pond.Model;
using Console = EzConsole.EzConsole;

namespace Pond.Core
{
    public static class PondCompiler
    {
        /// <summary>
        /// Generates the file structure from config file
        /// </summary>
        /// <param name="projectDirectory">Project directory</param>
        /// <param name="config">Configuration</param>
        public static void Compile(string projectDirectory, Config config, bool debug = false)
        {
            string articlesPath = Path.Combine(projectDirectory, config.ArticlesPath);
            string templatesPath = Path.Combine(projectDirectory, config.TemplatesPath);
            string stylesPath = Path.Combine(projectDirectory, config.StylesPath);
            string outputPath = Path.Combine(projectDirectory, config.OutputPath);
            
            if (!Directory.Exists(articlesPath)) Console.WriteLine("Articles directory could not be found!", ConsoleColor.Red);
            else if (!Directory.Exists(templatesPath)) Console.WriteLine("Templates directory could not be found!", ConsoleColor.Red);
            else if (!Directory.Exists(stylesPath)) Console.WriteLine("Styles directory could not be found!", ConsoleColor.Red);
            else
            {
                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

                #region Compile Articles
                
                void CompileArticleDirectory(string directoryPath, string relativePath)
                {
                    // Compile all articles using the templates.
                    string[] articles = Directory.GetFiles(directoryPath);
                    foreach (var article in articles)
                    {
                        ArticleHeadInfo ahi = ArticleHeadInfoParser.Parse(article, config.DefaultArticleInfo);

                        string fileOutput = Path.Combine(outputPath, Path.Combine(relativePath, Path.GetFileNameWithoutExtension(article))) + ".html";
                        Console.WriteLine($"Compiling '{article}' to '{fileOutput}' using template '{ahi.TemplateFileName()}'...", ConsoleColor.Yellow);
                        
                        string template = String.Empty;
                        if (File.Exists(Path.Combine(templatesPath, ahi.Template))) template = Path.Combine(templatesPath, ahi.Template);
                        else if (File.Exists(Path.Combine(templatesPath, ahi.TemplateFileName()))) template = Path.Combine(templatesPath, ahi.TemplateFileName());
                        else
                        {
                            Console.WriteLine($"Template '{ahi.TemplateFileName()}' not found in '{templatesPath}'! Please enter a valid filepath.", ConsoleColor.Red);
                            continue;
                        }

                        ArticleData ad = ArticleParser.Parse(article, ahi, debug);

                        TemplateAssembler.InitializeInterpreter(ad);
                        string result = TemplateAssembler.AssembleFile(ad, template);

                        File.WriteAllText(fileOutput, result);
                    }

                    string[] parts = Directory.GetDirectories(directoryPath);
                    foreach (string part in parts) CompileArticleDirectory(Path.Combine(directoryPath, part), part);
                }

                CompileArticleDirectory(articlesPath, string.Empty);

                #endregion

                #region Move Styles

                 

                #endregion
            }
        }
    }
}
