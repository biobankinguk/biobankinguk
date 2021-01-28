using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biobanks.Directory.Data.Constants
{
    public static class ConfigKey
    {
        public const string ShowPreservationPercentage = "site.display.preservation.percent";
        public const string ShowCounties = "site.display.counties";
        public const string DisplayLiveChat = "site.display.livechat";
        public const string DisplayAnalytics = "site.display.analytics";
        public const string DisplayPublications = "site.display.publications";
        public const string DisplayAboutPage = "site.display.aboutpage";

        public const string FundersFreeText = "site.display.funders.freetext";

        public const string ContactThirdParty = "site.display.thirdparty";

        //Email Config
        public const string RegistrationEmails = "site.email.registration";

        //Trusted Biobank Config
        public const string TrustBiobanks = "site.display.trusted.biobanks";

        // Editable Homepage
        public const string HomepageTitle = "site.homepage.title";
        public const string HomepageSearchTitle = "site.homepage.searchtitle";
        public const string HomepageSearchSubTitle = "site.homepage.searchsubtitle";
        public const string HomepageResourceRegistration = "site.homepage.resourceregistration";
        public const string HomepageNetworkRegistration = "site.homepage.networkregistration";
        public const string HomepageSearchRadioSamplesCollected = "site.homepage.searchradiosamplescollected";
        public const string HomepageSearchRadioAccessSamples = "site.homepage.searchradioaccesssamples";

        // Editable Register
        public const string RegisterBiobankTitle = "site.register.biobank.title";
        public const string RegisterBiobankDescription = "site.register.biobank.description";
        public const string RegisterNetworkTitle = "site.register.network.title";
        public const string RegisterNetworkDescription = "site.register.network.description";
        public const string EnableRegisterRegistrationHelpUrl = "site.register.help.show";
        public const string RegisterRegistrationHelpUrl = "site.register.help.url";

        //Editable Termspage
        public const string TermpageInfo = "site.termpage.pageinfo";


        // Ref Data Name Configuration Options
        public const string StorageTemperatureName = "site.display.preservation.name"; //TODO: Migrate key name?
        public const string DonorCountName = "site.display.donorcount.name";
        public const string MacroscopicAssessmentName = "site.display.macroscopicassessment.name";

        //Sample Resource Configuration Options
        public const string EnableHTA = "site.sampleresource.hta";
        public const string EnableDataSharing = "site.sampleresource.datasharing";
        public const string EthicsFieldName = "site.sampleresource.ethics.name";
        public const string EthicsFieldIsCheckbox = "site.sampleresource.ethics.type";
        public const string SampleResourceName = "site.sampleresource.name";

        // Editable About Page
        public const string AboutBodyText = "site.aboutpage.bodytext";

    }
}
