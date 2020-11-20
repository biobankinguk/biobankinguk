using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Biobanks.Web.HtmlHelpers
{
    public static class EnvironementHelper
    {
        public static string EnvironmentName(this HtmlHelper helper)
        {
            return ConfigurationManager.AppSettings["EnvironmentName"] ?? "Production";
        }

        public static bool IsProductionEnvironment(this HtmlHelper helper)
        {
            return helper.EnvironmentName() == "Production";
        }
    }
}