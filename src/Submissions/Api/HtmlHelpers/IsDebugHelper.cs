using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Biobanks.Web.HtmlHelpers
{
    public static class IsDebugHelper
    {
        public static bool IsDebug(this HtmlHelper helper)
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}