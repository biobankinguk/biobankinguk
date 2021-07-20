using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.HtmlHelpers
{
    public static class DisabledHelper
    {
        public static HtmlString DisabledIf(this HtmlHelper html, bool condition)
            => new HtmlString(condition ? "disabled=\"disabled\"" : "");
    }
}