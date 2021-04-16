using System.Collections.Generic;
using Biobanks.Entities.Data;

namespace Biobanks.Directory.Data.Configs
{
    public static class Configs
    {
        public static readonly List<Config> DefaultConfigs = new List<Config>
        {
            #region Display
            // Show About Page
            new Config
            {
                Key = "site.display.aboutpage",
                Value = "true",
                Name = "Display About Page",
                Description = "Enable/Disable About page",
                ReadOnly = false
            },
            // Show Analytics View
            new Config
            {
                Key = "site.display.analytics",
                Value = "true",
                Name = "Show Analytics",
                Description= "Enable/Disable Analytics View",
                ReadOnly = false
            },
            // Show Biobank Network Approval
            new Config
            {
                Key = "site.display.trusted.biobanks",
                Value = "true",
                Name = "Require Biobank Approval",
                Description = "Enable/Disable Biobank approval to join a Network",
                ReadOnly = false
            },
            // Show Counties
            new Config
            {
                Key = "site.display.counties",
                Value = "true",
                Name = "Use counties",
                Description = "Enable/Disable counties",
                ReadOnly = true
            },
            // Show Funders Free Text
            new Config
            {
                Key = "site.display.funders.freetext",
                Value = "false",
                Name = "Funders Free Text Input",
                Description = "Enable/Disable free text input for Funders",
                ReadOnly = false
            },
            // Show Live Chat
            new Config 
            {
                Key = "site.display.livechat",
                Value = "true",
                Name = "Show live chat",
                Description= "Enable/Disable live chat on all pages",
                ReadOnly = false
            },
            // Show Publications View
            new Config 
            {
                Key = "site.display.publications",
                Value = "true",
                Name = "Show Publications",
                Description= "Enable/Disable Publications View",
                ReadOnly = false
            },
            // Show Preservation Type Percentage
            new Config 
            {
                Key = "site.display.preservation.percent",
                Value = "true",
                Name = "Show preservation detail percentage",
                Description= "Enable/Disable preservation detail percentage",
                ReadOnly = false
            },
            // Show Third Party Contact Link
            new Config
            {
                Key = "site.display.thirdparty",
                Value = "true",
                Name = "Allow third party contact",
                Description = "Enable/Disable third party contact in Contact List",
                ReadOnly = false
            },
            // Storage Temperature Name Override
            new Config 
            {
                Key = "site.display.storagetemperature.name",
                Value = "Storage Temperature",
                Name = "Set Storage Temperatures Custom Name",
                Description= "Set Storage Temperatures Custom Name",
                ReadOnly = true
            },
            // Donor Count Name Override
            new Config 
            {
                Key = "site.display.donorcount.name",
                Value = "Donor Count",
                Name = "Set Donor Count Custom Name",
                Description= "Set Donor Count Custom Name",
                ReadOnly = true
            },
            // Macroscopic Assessment Name Override
            new Config
            {
                Key = "site.display.macroscopicassessment.name",
                Value = "Macroscopic Assessment",
                Name = "Set Macroscopic Assessments Custom Name",
                Description= "Set Macroscopic Assessments Custom Name",
                ReadOnly = true
            },
            #endregion
            
            #region Home Page
            new Config
            {
                Key = "site.homepage.title",
                Value = "Biobanking Directory",
            },
            new Config
            {
                Key = "site.homepage.searchtitle",
                Value = "Search the Directory",
            },
            new Config
            {
                Key = "site.homepage.searchsubtitle",
                Value = "Search for existing banked samples or for a group to collect samples on your behalf",
            },
            new Config
            {
                Key = "site.homepage.resourceregistration",
                Value = @"###Registering a sample resource

                        A sample resource is any infrastructure that holds or can collect and distribute human samples and data (e.g. biobanks, bioresources, biorepositories, cohorts and clinical trials).

                        ####For more information

                        We have developed some resources to help you register:

                        * [View our help pages](https://www.biobankinguk.org/directory/)
                        * [View our videos](https://youtu.be/7mnjyUwVuTA)",
            },
            new Config
            {
                Key = "site.homepage.networkregistration",
                Value = @"###Registering a network
                        A network is a group of sample resources that have come together with some common objective or agreed standard. An example is the [Confederation of Cancer Biobanks](https://directory.biobankinguk.org/Profile/Network/2).

                        ####Sample resource or Network?
                        If you are unsure about whether to register a resource or network then [please get in touch](https://www.biobankinguk.org/contact-us/).",
            },
            new Config
            {
                Key = "site.homepage.searchradiosamplescollected",
                Value = "For a group to collect samples on your behalf",
                Description = "Set require samples collected text",
                ReadOnly = false,
            },
            new Config
            {
                Key = "site.homepage.searchradioaccesssamples",
                Value = "Access existing samples",
                Description = "Set access existing samples text",
                ReadOnly = false,
            },
            #endregion

            #region Register Page
            new Config
            {
                Key = "site.register.biobank.title",
                Value = "Register a new sample resource"
            },
            new Config
            {
                Key = "site.register.biobank.description",
                Value = "If you have a collection of tissue samples or the ability to collect samples for researchers then you can add an overview to our database by registering below.",
            },
            new Config
            {
                Key = "site.register.network.title",
                Value = "Register a new network"
            },
            new Config
            {
                Key = "site.register.network.description",
                Value = "If you are a network, such as the Confederation of Cancer Biobanks, you can register below.",
            },
            new Config
            {
                Key = "site.register.help.show",
                Value = "false",
            },
            new Config
            {
                Key = "site.register.help.url",
                Value = "https://biobankinguk.org/share/",
            },
            #endregion

            #region SampleResource Page
            // Show HTA
            new Config
            {
                Key = "site.sampleresource.hta",
                Value = "true",
                Name = "Use Hta",
                Description = "Enable/Disable Hta",
                ReadOnly = false
            },
            // Show Data Sharing 
            new Config
            {
                Key = "site.sampleresource.datasharing",
                Value = "true",
                Name = "Use Data Sharing",
                Description = "Enable/Disable data sharing",
                ReadOnly = false
            },
            // Ethics Field Name
            new Config
            {
                Key = "site.sampleresource.ethics.name",
                Value = "Ethics Committee Approval",
                Name = "Ethics Field Name",
                Description = "Set ethics field name",
                ReadOnly = false
            },
            // Sample Resource Name
            new Config
            {
                Key = "site.sampleresource.name",
                Value = "Sample Resource",
                Name = "Sample Resource Name",
                Description = "Set sample resource name",
                ReadOnly = false
            },
            new Config
            {
                Key = "site.sampleresource.ethics.type",
                Value = "true",
                Name = "Ethics Field Type",
                Description = "If set to true ethics field type is a checkbox, else free text",
                ReadOnly = false
            },
            #endregion
            
            // Enable/Disable Registration Emails
            new Config
            {
                Key = "site.email.registration",
                Value = "true",
                Name = "Allow registration emails to be sent",
                Description = "Enable/Disable whether registration emails are to be sent",
                ReadOnly = false
            },
            // Term Page Info
            new Config
            {
                Key = "site.termpage.pageinfo",
                Value = @"This page is designed to help you find sample resources using SNOMED CT terms. We are committed to using established standards. SNOMED CT is currently being rolled out across the NHS and therefore we have adopted these [terms for disease classifications](https://biobankinguk.org/snomed-ct-using/).

We do appreciate that not everyone will be aware of the terms used on the FIND Virtual Biobank Directory(VBD). The table below captures all the terms that sample resources have used in the Directory so far. Simply use the search to filter the list and then click on 'Find Biobanks'.",
            },
            // About Page Body Text
            new Config {
                Key = "site.aboutpage.bodytext",
                Value = @"##About Us"
            }
        };
    }
}
