using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.HtmlHelpers
{
    public static class VersionHelper
    {
        public static string AssemblyVersion(this HtmlHelper helper)
        {
            return Utilities.VersionHelper.GetAspNetAssembly().GetName().Version.ToString();
        }

        public static string InformationalVersion(this HtmlHelper helper)
        {
            //InformationalVersion can contain strings, not just numbers
            //so is useful for Semantic Versioning http://semver.org            

            var attr = Utilities.VersionHelper.GetVersionNumber();            

            //return the assembly version if no informational version
            return attr ?? AssemblyVersion(helper);
        }        
    }
}