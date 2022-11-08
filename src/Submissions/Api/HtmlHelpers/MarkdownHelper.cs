﻿using MarkdownDeep;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Biobanks.Submissions.Api.HtmlHelpers
{
    public static class MarkdownHelper
    {
        public static HtmlString Markdown(this IHtmlHelper helper, string markdown)
        {
            var markdownTransformed = new Markdown
            {
                ExtraMode = true,
                MarkdownInHtml = true
                //SafeMode = true
            }.Transform(markdown);

            return new HtmlString(
                markdownTransformed
             );
        }
    }
}
