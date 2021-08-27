using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Biobanks.Directory.Data;
using Biobanks.Directory.Data.Caching;
using Biobanks.Identity.Data;
using Biobanks.Directory.Data.Repositories;
using Biobanks.Identity.Services;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Search.Legacy;
using Biobanks.Services;
using Biobanks.Services.Contracts;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using Biobanks.Search.Contracts;
using Biobanks.Search.Elastic;
using System.Web.Http;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Directory.Services;

namespace Biobanks.Web.Windsor
{
    /// <summary>
    /// Windsor Installer for MVC bits
    /// </summary>
    public class WindsorInstaller : IWindsorInstaller
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public WindsorInstaller(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // We know that it's not considered best practice to register the container, but we need to inject it
            // into the WindsorActionInvoker
            container.Register(
                Component.For<IWindsorContainer>().Instance(container),
                Component.For<IActionInvoker>().ImplementedBy<WindsorActionInvoker>().LifeStyle.Singleton
            );


            // Config / value dependencies we instantiate here
            var dbConnectionString = ConfigurationManager.ConnectionStrings["Biobanks"].ConnectionString;

            var sendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"];

            var mapper = AutoMapperConfig.RegisterMappings().CreateMapper();


            // Service registrations
            container.Register(
                Component.For(typeof(IGenericEFRepository<>))
                    .ImplementedBy(typeof(GenericEFRepository<>))
                    .LifeStyle.Transient,

                Component.For<UserStoreDbContext>()
                    .DependsOn(Dependency.OnValue("connectionString", dbConnectionString))
                    .LifestylePerWebRequest(),

                Component.For<BiobanksDbContext>()
                    .DependsOn(Dependency.OnValue("connectionString", dbConnectionString))
                    .LifestylePerWebRequest(),

                Component
                    .For
                    <IApplicationUserManager<ApplicationUser, string, IdentityResult>,
                        UserManager<ApplicationUser, string>>()
                    .ImplementedBy<ApplicationUserManager>()
                    .DependsOn(Dependency.OnValue("dataProtectionProvider", _dataProtectionProvider))
                    .LifeStyle.Transient,

                Component.For<CustomClaimsManager>()
                    .LifeStyle.Transient,

                Component.For<ApplicationSignInManager>()
                    .LifeStyle.Transient,

                Component.For<IApplicationRoleManager<ApplicationRole>, ApplicationRoleManager>()
                    .LifeStyle.Transient,

                Component.For<IRoleStore<ApplicationRole, string>>()
                    .ImplementedBy<ApplicationRoleStore>()
                    .LifeStyle.Transient,

                Component.For<ICacheProvider>()
                    .ImplementedBy<CacheProvider>()
                    .LifeStyle.Singleton,

                Component.For<IUserStore<ApplicationUser, string>>()
                    .ImplementedBy<ApplicationUserStore>()
                    .LifeStyle.Transient,

                Component.For(typeof(IOntologyTermService))
                    .ImplementedBy(typeof(OntologyTermService))
                    .LifeStyle.Transient,

                Component.For(typeof(IBiobankReadService))
                    .ImplementedBy(typeof(BiobankReadService))
                    .LifeStyle.Transient,

                Component.For(typeof(IBiobankWriteService))
                    .ImplementedBy(typeof(BiobankWriteService))
                    .LifeStyle.Transient,

                Component.For(typeof(ICollectionService))
                    .ImplementedBy(typeof(CollectionService))
                    .LifeStyle.Transient,

                Component.For(typeof(IContentPageService))
                    .ImplementedBy(typeof(ContentPageService))
                    .LifeStyle.Transient,

                Component.For(typeof(IConfigService))
                    .ImplementedBy(typeof(ConfigService))
                    .LifeStyle.Transient,

                Component.For(typeof(IRegistrationDomainService))
                    .ImplementedBy(typeof(RegistrationDomainService))
                    .LifeStyle.Transient,

                // Doubled up For because Hangfire requires both concrete class and interface.
                Component.For<ElasticCollectionSearchProvider, ICollectionSearchProvider>()
                    .ImplementedBy<ElasticCollectionSearchProvider>()
                    .DependsOn(Dependency.OnValue("elasticSearchUrl",
                        ConfigurationManager.AppSettings["ElasticSearchUrl"]))
                    .DependsOn(Dependency.OnValue("indexNames", (
                        collections: ConfigurationManager.AppSettings["DefaultCollectionsSearchIndex"],
                        capabilities: ConfigurationManager.AppSettings["DefaultCapabilitiesSearchIndex"]))
                    )
                    .DependsOn(Dependency.OnValue("username", ConfigurationManager.AppSettings["ElasticSearchUsername"]))
                    .DependsOn(Dependency.OnValue("password", ConfigurationManager.AppSettings["ElasticSearchPassword"]))
                    .LifeStyle.Transient,
                Component.For<ElasticCollectionIndexProvider, ICollectionIndexProvider>()
                    .ImplementedBy<ElasticCollectionIndexProvider>()
                    .DependsOn(Dependency.OnValue("elasticSearchUrl",
                        ConfigurationManager.AppSettings["ElasticSearchUrl"]))
                    .DependsOn(Dependency.OnValue("indexNames", (
                        collections: ConfigurationManager.AppSettings["DefaultCollectionsSearchIndex"],
                        capabilities: ConfigurationManager.AppSettings["DefaultCapabilitiesSearchIndex"]))
                    )
                    .DependsOn(Dependency.OnValue("username", ConfigurationManager.AppSettings["ElasticSearchUsername"]))
                    .DependsOn(Dependency.OnValue("password", ConfigurationManager.AppSettings["ElasticSearchPassword"]))
                    .LifeStyle.Transient,

                Component.For<ElasticCapabilitySearchProvider, ICapabilitySearchProvider>()
                    .ImplementedBy<ElasticCapabilitySearchProvider>()
                    .DependsOn(Dependency.OnValue("elasticSearchUrl",
                        ConfigurationManager.AppSettings["ElasticSearchUrl"]))
                    .DependsOn(Dependency.OnValue("indexNames", (
                        collections: ConfigurationManager.AppSettings["DefaultCollectionsSearchIndex"],
                        capabilities: ConfigurationManager.AppSettings["DefaultCapabilitiesSearchIndex"]))
                    )
                    .DependsOn(Dependency.OnValue("username", ConfigurationManager.AppSettings["ElasticSearchUsername"]))
                    .DependsOn(Dependency.OnValue("password", ConfigurationManager.AppSettings["ElasticSearchPassword"]))
                    .LifeStyle.Transient,
                Component.For<ElasticCapabilityIndexProvider, ICapabilityIndexProvider>()
                    .ImplementedBy<ElasticCapabilityIndexProvider>()
                    .DependsOn(Dependency.OnValue("elasticSearchUrl",
                        ConfigurationManager.AppSettings["ElasticSearchUrl"]))
                    .DependsOn(Dependency.OnValue("indexNames", (
                        collections: ConfigurationManager.AppSettings["DefaultCollectionsSearchIndex"],
                        capabilities: ConfigurationManager.AppSettings["DefaultCapabilitiesSearchIndex"]))
                    )
                    .DependsOn(Dependency.OnValue("username", ConfigurationManager.AppSettings["ElasticSearchUsername"]))
                    .DependsOn(Dependency.OnValue("password", ConfigurationManager.AppSettings["ElasticSearchPassword"]))
                    .LifeStyle.Transient,

                Component.For<LegacySearchProvider,ISearchProvider>()
                    .ImplementedBy<LegacySearchProvider>()
                    .LifeStyle.Transient,
                Component.For<LegacyIndexProvider, IIndexProvider>()
                    .ImplementedBy<LegacyIndexProvider>()
                    .LifeStyle.Transient,


                Component.For<BiobankIndexService, IBiobankIndexService>()
                    .ImplementedBy(typeof(BiobankIndexService))
                    .LifeStyle.Transient,

                Component.For<IAuthenticationManager>().UsingFactoryMethod(() =>
                {
                    if (HttpContext.Current != null) return HttpContext.Current.GetOwinContext().Authentication;
                    throw new ApplicationException(
                        "No current HttpContext means I cannot inject an IAuthenticationManager");
                }).LifeStyle.Transient,

                Component.For<IMapper>()
                    .Instance(mapper)
                    .LifeStyle.Singleton,

                Component.For<TokenLoggingService, ITokenLoggingService>()
                    .ImplementedBy(typeof(TokenLoggingService))
                    .LifeStyle.Transient,

                Component.For<HttpClient>()
                    .OnCreate(client =>
                    {
                        client.BaseAddress = new Uri("https://www.google.com/recaptcha/api/siteverify");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    })
                    .LifeStyle.Singleton
            );

            // WebAPI
            container.Register(
                Classes.FromThisAssembly()
                        .BasedOn<ApiController>()
                        .LifestylePerWebRequest());

            // Email
            container.Register(
                    Component.For(typeof(Postal.IEmailService))
                    .ImplementedBy(typeof(Postal.EmailService))
                    .LifeStyle.Transient);

            if (!string.IsNullOrWhiteSpace(sendGridApiKey))
            {
                container.Register(
                    Component.For(typeof(IEmailService))
                    .ImplementedBy(typeof(SendGridEmailService))
                    .DependsOn(Dependency.OnValue("apiKey", sendGridApiKey))
                    .DependsOn(Dependency.OnValue("fromAddress", ConfigurationManager.AppSettings["EmailFromAddress"]))
                    .DependsOn(Dependency.OnValue("fromName", ConfigurationManager.AppSettings["EmailFromName"]))
                    .DependsOn(Dependency.OnValue("directoryName", ConfigurationManager.AppSettings["DirectoryName"]))
                    .LifeStyle.Transient);
            }
            else
            {
                container.Register(
                    Component.For(typeof(IEmailService))
                    .ImplementedBy(typeof(EmailService))
                    .LifeStyle.Transient);
            }


            container.Register(
                Component.For(typeof(ILogoStorageProvider))
                    .ImplementedBy(typeof(SqlServerLogoStorageProvider))
                    .LifeStyle.Transient);

            //Google Analytics 
            container.Register(
                    Component.For(typeof(IAnalyticsReportGenerator))
                    .ImplementedBy(typeof(AnalyticsReportGenerator))
                    .LifeStyle.Transient);

        }
    }
}
