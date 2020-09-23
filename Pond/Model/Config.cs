namespace Pond.Model
{
    public class Config
    {
        public static Config Default = new Config("articles", "templates", "style", "site", new ArticleHeadInfo("index"));

        public string ArticlesPath, TemplatesPath, StylesPath, OutputPath;
        public ArticleHeadInfo DefaultArticleInfo;

        public Config(string articlesPath, string templatesPath, string stylesPath, string outputPath, ArticleHeadInfo defaultArticleInfo)
        {
            ArticlesPath = articlesPath;
            TemplatesPath = templatesPath;
            StylesPath = stylesPath;
            OutputPath = outputPath;
            DefaultArticleInfo = defaultArticleInfo;
        }
    }
}
