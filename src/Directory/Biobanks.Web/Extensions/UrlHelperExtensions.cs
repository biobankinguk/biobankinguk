using System.Web.Mvc;

namespace Biobanks.Web.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string Blob(this UrlHelper helper, string resource)
            => helper.Action("Index", "Blob", new { resourceName = resource });
    }
}