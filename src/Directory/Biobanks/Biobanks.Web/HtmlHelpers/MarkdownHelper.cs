using System.Web;
using System.Web.Mvc;
using MarkdownDeep;

namespace Biobanks.Web.HtmlHelpers
{
    public static class MarkdownHelper
    {
        public static IHtmlString Markdown(this HtmlHelper helper, string markdown)
        {
            return MvcHtmlString.Create(
                new Markdown
                {
                    ExtraMode = true,
                    MarkdownInHtml = true
                    //SafeMode = true
                }.Transform(markdown));
        }
    }
}