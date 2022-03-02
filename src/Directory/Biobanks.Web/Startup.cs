using System;
using System.Configuration;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;

using Biobanks.Web.Filters;
using Biobanks.Web.Windsor;
using Biobanks.Web.Windsors;

using Castle.Windsor;

using Hangfire;
using Hangfire.Windsor;
using Microsoft.Owin.Security.DataProtection;


using Owin;


namespace Biobanks.Web
{
    public partial class Startup
    {
        //Stick this here so we can get to it when using DI
        //http://tech.trailmax.info/2014/09/aspnet-identity-and-ioc-container-registration/
        internal static IDataProtectionProvider DataProtectionProvider { get; private set; }

        [Obsolete("Sections of this method contain Obselete usages. Reference the comments for details")]
        // Hangfire AuthorizationFilters
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

            #region Hangfire

            var hangfireConnectionString = ConfigurationManager.ConnectionStrings["Hangfire"].ConnectionString;
            GlobalConfiguration.Configuration.UseSqlServerStorage(
                !string.IsNullOrWhiteSpace(hangfireConnectionString)
                    ? hangfireConnectionString
                    : ConfigurationManager.ConnectionStrings["Biobanks"].ConnectionString,
                new Hangfire.SqlServer.SqlServerStorageOptions
                {
                    SchemaName = ConfigurationManager.AppSettings["Hangfire__SchemaName"]
                });

            GlobalConfiguration.Configuration.UseWindsorActivator(windsorContainer.Kernel);

            // Make sure only SuperUsers can access the Hangfire dashboard.
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            // Start the Hangfire services.
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            #endregion
        }
    }
}
