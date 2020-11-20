using System;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Biobanks.Web.App_Start;
using Castle.Windsor;
using Hangfire;
using Microsoft.ApplicationInsights.Extensibility;
using GlobalConfiguration = System.Web.Http.GlobalConfiguration;

namespace Biobanks.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static IWindsorContainer WindsorContainer { get; set; }

        static MvcApplication()
        {
            //WindsorContainer = new WindsorContainer();
            //WindsorContainer.Install(new WindsorInstaller(), new CoreInstaller());
        }

        protected void Application_Start()
        {
            ProfilerConfig.Configure();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register); // API must be routed before MVC
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true; //When two tabs go to war, one login is all that they can score

            //ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(WindsorContainer));
        }

        protected void Application_BeginRequest()
        {
            ProfilerConfig.Start();
        }

        protected void Application_EndRequest()
        {
            ProfilerConfig.Stop();
        }
    }
}
