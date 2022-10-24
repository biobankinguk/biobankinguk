using Microsoft.AspNetCore.Http;
using System;
using System.Reflection;

namespace Biobanks.Submissions.Api.Utilities
{
    public class VersionHelper
    {
        public static string GetInformationalVersion()
        {
            //InformationalVersion can contain strings, not just numbers
            //so is useful for Semantic Versioning http://semver.org 
            //http://stackoverflow.com/questions/7770068/get-the-net-assemblys-assemblyinformationalversion-value
            var attr = Attribute.GetCustomAttribute(
                    GetAspNetAssembly(),
                    typeof(AssemblyInformationalVersionAttribute))
                as AssemblyInformationalVersionAttribute;

            return attr?.InformationalVersion;
        }

        public static Assembly GetAspNetAssembly()
        {
            //http://stackoverflow.com/questions/4277692/getentryassembly-for-web-applications
            return HttpContext.Current.ApplicationInstance.GetType().BaseType?.Assembly;
        }
    }
}
