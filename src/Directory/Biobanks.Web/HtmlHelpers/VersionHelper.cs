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
            return GetAspNetAssembly().GetName().Version.ToString();
        }

        public static string InformationalVersion(this HtmlHelper helper)
        {
            //InformationalVersion can contain strings, not just numbers
            //so is useful for Semantic Versioning http://semver.org

            //http://stackoverflow.com/questions/7770068/get-the-net-assemblys-assemblyinformationalversion-value
            var attr = Attribute.GetCustomAttribute(
                    GetAspNetAssembly(),
                    typeof(AssemblyInformationalVersionAttribute))
                as AssemblyInformationalVersionAttribute;

            //return the assembly version if no informational version
            return attr?.InformationalVersion ?? AssemblyVersion(helper);
        }

        private static Assembly GetAspNetAssembly()
        {
            //http://stackoverflow.com/questions/4277692/getentryassembly-for-web-applications
            return HttpContext.Current.ApplicationInstance.GetType().BaseType?.Assembly;
        }
    }
}