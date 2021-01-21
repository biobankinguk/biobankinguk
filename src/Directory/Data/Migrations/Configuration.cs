using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Text;
using Directory.Entity.Data;
using Newtonsoft.Json;
using System.Data.Entity.Migrations;
using Directory.Data.Transforms.Url;
using Directory.Data.Constants;

namespace Directory.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<BiobanksDbContext>
    {
        public Configuration()
        {
            // genuinely not sure what the default is, so let's be explicit.
            AutomaticMigrationsEnabled = false;

            // Leaving this the same as the original Migrations namespace forever
            // Makes changing the namespace easier without any faff in existing DBs
            ContextKey = "Biobanks.Data.Migrations.Configuration";
        }

        protected override void Seed(BiobanksDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            #region Org Type
            // This can stay seeded here.
            // It never changes.
            // It's not really reference data in the new world
            // It's a directory specific lookup that is actually redundant
            // (there has only ever been one type)
            // but it's less effort to leave here than remove, for now
            // TODO: remove and refactor

            new List<OrganisationType>
                    {
                        new OrganisationType
                        {
                            OrganisationTypeId = 1,
                            Description = "Biobank",
                            SortOrder = 1
                        }
                    }.ForEach(organisationType =>
                        context.OrganisationTypes.AddOrUpdate(x => x.OrganisationTypeId, organisationType));

            #endregion

            #region Site Config

            var defaultConfig = new List<Config>
                    {
                        // Show Counties
                        new Config
                        {
                            Key = "site.display.thirdparty",
                            Value = "true",
                            Name = "Allow third party contact",
                            Description = "Enable/Disable third party contact in Contact List",
                            ReadOnly = false
                        },

                        // Enable/Disable Registration Emails
                        new Config
                        {
                            Key = "site.email.registration",
                            Value = "true",
                            Name = "Allow registration emails to be sent",
                            Description = "Enable/Disable whether registration emails are to be sent",
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

                        // Live Chat Config
                        new Config {
                            Key = "site.display.livechat",
                            Value = "true",
                            Name = "Show live chat",
                            Description= "Enable/Disable live chat on all pages",
                            ReadOnly = false
                        },

                        // Analytics Config
                        new Config {
                            Key = "site.display.analytics",
                            Value = "true",
                            Name = "Show Analytics",
                            Description= "Enable/Disable Analytics View",
                            ReadOnly = false
                        },

                        // Publications Config
                        new Config {
                            Key = "site.display.publications",
                            Value = "true",
                            Name = "Show Publications",
                            Description= "Enable/Disable Publications View",
                            ReadOnly = false
                        },

                        // Preservation Details
                        new Config {
                            Key = "site.display.preservation.percent",
                            Value = "true",
                            Name = "Show preservation detail percentage",
                            Description= "Enable/Disable preservation detail percentage",
                            ReadOnly = false
                        },

                        // Preservation Details Name
                        new Config {
                            Key = "site.display.preservation.name",
                            Value = "Preservation Type",
                            Name = "Set Preservation Details Custom Name",
                            Description= "Set Preservation Details Custom Name",
                            ReadOnly = true
                        },

                        // Donor Count Name
                        new Config {
                            Key = "site.display.donorcount.name",
                            Value = "Donor Count",
                            Name = "Set Donor Count Custom Name",
                            Description= "Set Donor Count Custom Name",
                            ReadOnly = true
                        },

                        // Macroscopic Assessment Name
                        new Config {
                            Key = "site.display.macroscopicassessment.name",
                            Value = "Macroscopic Assessment",
                            Name = "Set Macroscopic Assessments Custom Name",
                            Description= "Set Macroscopic Assessments Custom Name",
                            ReadOnly = true
                        },

                        // Funders Free Text
                        new Config
                        {
                            Key = "site.display.funders.freetext",
                            Value = "false",
                            Name = "Funders Free Text Input",
                            Description = "Enable/Disable free text input for Funders",
                            ReadOnly = false
                        },

                        // Homepage Config
                        new Config {
                            Key = "site.homepage.title",
                            Value = "Biobanking Directory",
                        },
                        new Config {
                            Key = "site.homepage.searchtitle",
                            Value = "Search the Directory",
                        },
                        new Config {
                            Key = "site.homepage.resourceregistration",
                            Value = @"###Registering a sample resource

                                    A sample resource is any infrastructure that holds or can collect and distribute human samples and data (e.g. biobanks, bioresources, biorepositories, cohorts and clinical trials).

                                    ####For more information

                                    We have developed some resources to help you register:

                                    * [View our help pages](https://www.biobankinguk.org/directory/)
                                    * [View our videos](https://youtu.be/7mnjyUwVuTA)",
                        },
                        new Config {
                            Key = "site.homepage.networkregistration",
                            Value = @"###Registering a network
                                    A network is a group of sample resources that have come together with some common objective or agreed standard. An example is the [Confederation of Cancer Biobanks](https://directory.biobankinguk.org/Profile/Network/2).

                                    ####Sample resource or Network?
                                    If you are unsure about whether to register a resource or network then [please get in touch](https://www.biobankinguk.org/contact-us/).",
                        },
                        new Config
                        {
                            Key = "site.homepage.searchradiosamplescollected",
                            Value = "Require samples collected",
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

                        /** Register Page Config **/
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

                        /** Sample Resource Config**/

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

                        // Ethics
                        new Config
                        {
                            Key = "site.sampleresource.ethics.name",
                            Value = "Ethics Committee Approval",
                            Name = "Ethics Field Name",
                            Description = "Set ethics field name",
                            ReadOnly = false
                        },
                        //Sample Resource Name
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

                        new Config
                        {
                            Key = "site.termpage.pageinfo",
                            Value = @"This page is designed to help you find sample resources using SNOMED CT terms. We are committed to using established standards. SNOMED CT is currently being rolled out across the NHS and therefore we have adopted these [terms for disease classifications](https://biobankinguk.org/snomed-ct-using/).

        We do appreciate that not everyone will be aware of the terms used on the FIND Virtual Biobank Directory(VBD). The table below captures all the terms that sample resources have used in the Directory so far. Simply use the search to filter the list and then click on 'Find Biobanks'.",
                        },
                        new Config
                        {
                            Key = "site.display.trusted.biobanks",
                            Value = "true",
                            Name = "Require Biobank Approval",
                            Description = "Enable/Disable Biobank approval to join a Network",
                            ReadOnly = false
                        },

                        // Aboutpage Config
                        new Config {
                            Key = "site.aboutpage.bodytext",
                            Value = @"##About Us"
                        },
                        new Config
                        {
                            Key = "site.display.aboutpage",
                            Value = "true",
                            Name = "Display About Page",
                            Description = "Enable/Disable About page",
                            ReadOnly = false
                        },
                    };

            context.Configs
                .AsEnumerable() // Moves database data into memory
                .Concat(defaultConfig)
                .GroupBy(x => x.Key)
                .Select(x => x.FirstOrDefault())
                .ToList()
                .ForEach(x => context.Configs.AddOrUpdate(x));

            #endregion

            #region Data fixes
            // This is actually genius abuse of the Seed functionality
            // so I'm leaving it here.

            //profile url fixes
            context.Organisations.ToList().ForEach(org =>
            {
                try
                {
                    org.Url = UrlTransformer.Transform(org.Url);
                            //this should fix up existing typo'd urls, such as http://http://www.cheese.com
                            //if it can't fix them it will leave them alone
                        }
                catch (Exception e) when (e is UriFormatException || e is InvalidUrlSchemeException)
                {
                            //do nothing in the event we don't like the currently stored url after attempting to fix it
                            //these ones will just have to be fixed manually
                        }
            });

            //Fix Site Config Data by removing intial seed
            var items = new List<string> {
                        "site.contact.thirdparty",
                        "site.input.funders.freetext",
                        "site.display.datasharing",
                        "site.display.hta"
                    };

            foreach (var item in items)
            {
                var itemToRemove = context.Configs.SingleOrDefault(x => x.Key == item); //returns a single item.

                if (itemToRemove != null)
                    context.Configs.Remove(itemToRemove);
            }


            #endregion

            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                ); // Add the original exception as the innerException
            }
        }
    }
}
