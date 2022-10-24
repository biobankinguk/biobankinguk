using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Biobanks.Submissions.Api.HtmlHelpers
{
    public static class VersionHelper
    {
        public static string AssemblyVersion(this HtmlHelper helper)
        {
            return Utilities.VersionHelper.GetAspNetAssembly().GetName().Version.ToString();
        }

        public static string InformationalVersion(this HtmlHelper helper)
        {
            //return the assembly version if no informational version
            return Utilities.VersionHelper.GetInformationalVersion() ?? helper.AssemblyVersion();
        }
    }
}