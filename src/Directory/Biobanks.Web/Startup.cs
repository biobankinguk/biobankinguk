using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Biobanks.Web.App_Start;
using Biobanks.Web.Filters;
using Biobanks.Web.HangfireJobActivator;
using Biobanks.Web.Windsor;
using Biobanks.Web.Windsors;
using Castle.Windsor;
using Hangfire;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace Biobanks.Web
{
    public partial class Startup
    {
        //Stick this here so we can get to it when using DI
        //http://tech.trailmax.info/2014/09/aspnet-identity-and-ioc-container-registration/
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            DataProtectionProvider = app.GetDataProtectionProvider();

            // Do Windsor initialisation here, so we can pass it OWIN stuff, like DataProtectionProvider.
            var windsorContainer = new WindsorContainer();

            windsorContainer.Install(new WindsorInstaller(DataProtectionProvider));
            MvcApplication.WindsorContainer = windsorContainer;
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(MvcApplication.WindsorContainer));

            // Requried to used WebAPIController with Windsor https://stackoverflow.com/questions/16154566/ioc-castle-windsor-and-webapi
            System.Web.Http.GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator), new WindsorCompositionRoot(windsorContainer));

            ConfigureAuth(app);

            GlobalConfiguration.Configuration.UseSqlServerStorage("Biobanks");

            JobActivator.Current = new HangfireWindsorJobActivator(windsorContainer.Kernel);

            // Make sure only SuperUsers can access the Hangfire dashboard.
            app.UseHangfireDashboard("/hangfire", new DashboardOptions { AuthorizationFilters = new[] { new HangFireAuthorizationFilter() } });

            // Start the Hangfire services.
            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
