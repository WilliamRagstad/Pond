namespace Pond.Model
{
    public class ArticleHeadInfo
    {
        /// <summary>
        /// Name of template to use for a specific article
        /// </summary>
        public string Template;

        public ArticleHeadInfo(string template)
        {
            Template = template;
        }

        public string TemplateFileName() => Template + ".html";
    }
}
