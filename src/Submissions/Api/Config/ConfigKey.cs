namespace Biobanks.Submissions.Api.Config
{
  public static class ConfigKey
  {
    // General Site Display Properties
    public const string DisplayLiveChat = "site.display.livechat";
    public const string DisplayAnalytics = "site.display.analytics";
    public const string ShowPreservationPercentage = "site.display.preservation.percent";
    public const string ShowCounties = "site.display.counties";
    public const string DisplayPublications = "site.display.publications";
    public const string DisplaySubmissions = "site.display.submissions";
    public const string CollectionsNotes = "site.display.collectionsnotes";
    public const string FundersFreeText = "site.display.funders.freetext";

    // Ref Data Name Configuration Options
    public const string MacroscopicAssessmentName = "site.display.macroscopicassessment.name";
    public const string DonorCountName = "site.display.donorcount.name";
    public const string StorageTemperatureName = "site.display.storagetemperature.name";

    //Email Config
    public const string RegistrationEmails = "site.email.registration";

    //Trusted Biobank Config
    public const string TrustBiobanks = "site.display.trusted.biobanks";

    // Editable Homepage
    public const string HomepageTitle = "site.homepage.title";
    public const string HomepageSearchTitle = "site.homepage.searchtitle";
    public const string HomepageSearchSubTitle = "site.homepage.searchsubtitle";
    public const string HomepageResourceRegistration = "site.homepage.resourceregistration";
    public const string HomepageResourceRegistration2 = "site.homepage.resourceregistration2";
    public const string HomepageNetworkRegistration = "site.homepage.networkregistration";
    public const string HomepageNetworkRegistration2 = "site.homepage.networkregistration2";
    public const string HomepageSearchRadioSamplesCollected = "site.homepage.searchradiosamplescollected";
    public const string HomepageSearchRadioAccessSamples = "site.homepage.searchradioaccesssamples";
    public const string HomepageFinalParagraph = "site.homepage.finalparagraph";

    // Editable Register
    public const string RegisterBiobankTitle = "site.register.biobank.title";
    public const string RegisterNetworkTitle = "site.register.network.title";
    public const string RegisterBiobankDescription = "site.register.biobank.description";
    public const string RegisterNetworkDescription = "site.register.network.description";
    public const string EnableRegisterRegistrationHelpUrl = "site.register.help.show";
    public const string RegisterRegistrationHelpUrl = "site.register.help.url";

    //Editable Termspage
    public const string TermpageInfo = "site.termpage.pageinfo";

    // Sample Resource Configuration Options
    public const string SampleResourceName = "site.sampleresource.name";
    public const string EnableDataSharing = "site.sampleresource.datasharing";
    public const string EthicsFieldName = "site.sampleresource.ethics.name";
    public const string EthicsFieldIsCheckbox = "site.sampleresource.ethics.type";

    // Sample Resource Configuration Options
    public const string ContactThirdParty = "site.display.thirdparty";
  }
}

