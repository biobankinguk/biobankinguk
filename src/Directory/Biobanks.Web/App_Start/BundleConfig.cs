using Castle.Windsor.Installer;
using System;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Web.Hosting;
using System.Web.Optimization;
using System.Web.WebPages;

namespace Biobanks.Web
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region Styles
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-theme.css",
                "~/Content/DataTables/css/dataTables.bootstrap.css",
                "~/Content/DataTables/css/rowReorder.bootstrap.min.css",
                "~/Content/DataTables/css/buttons.bootstrap.css",
                "~/Content/bootstrap-slider.min.css",
                "~/Content/typeahead.css",
                "~/Scripts/MarkdownDeep/mdd_styles.css",
                "~/Content/Site/*.css"));

            bundles.Add(new StyleBundle("~/Content/fontawesome").Include(
                "~/Content/font-awesome.css"));

            #endregion

            #region Scripts
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery-unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/bootstrap.fix.jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.cookie-1.4.1.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.mapping*"));

            bundles.Add(new ScriptBundle("~/bundles/typeahead").Include(
                      "~/Scripts/typeahead.bundle.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/session-timeout").Include(
                "~/Scripts/bootstrap-session-timeout/bootstrap-session-timeout*"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/DataTables/jquery.dataTables.js",
                "~/Scripts/moment*",
                "~/Scripts/DataTables/datetime-moment.js",
                "~/Scripts/DataTables/dataTables.bootstrap.js",
                "~/Scripts/DataTables/dataTables.buttons.js",
                "~/Scripts/DataTables/buttons.bootstrap.js",
                "~/Scripts/DataTables/buttons.html5.js",
                "~/Scripts/DataTables/buttons.print.js",
                "~/Scripts/DataTables/buttons.colVis.min.js",
                "~/Scripts/DataTables/dataTables.rowReorder.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootbox").Include(
                        "~/Scripts/bootbox*",
                        "~/Scripts/setup.bootbox.form.confirm.js"));

            bundles.Add(new ScriptBundle("~/bundles/markdowndeep").Include(
                "~/Scripts/MarkdownDeep/MarkDownDeepLib.min.js",
                "~/Scripts/Shared/mdd-editor.js"));

            //Home
            bundles.Add(new ScriptBundle("~/bundles/home").Include(
                "~/Scripts/Home/unified-search-helptext.js"));
            bundles.Add(new ScriptBundle("~/bundles/home/contact-list").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/Home/home-contactlist.js"));

            //Biobank Admin
            bundles.Add(new ScriptBundle("~/bundles/biobank/admins").Include(
                "~/Scripts/Shared/registerentity-admin-viewmodel.js",
                "~/Scripts/Biobank/admins-viewmodel.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/annualstats").Include(
                "~/Scripts/Biobank/annualstats.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/funders").Include(
                "~/Scripts/typeahead.bundle*",
                "~/Scripts/Biobank/funders-type-ahead.js",
                "~/Scripts/Biobank/funder-viewmodel.js",
                "~/Scripts/Biobank/funders-viewmodel.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/edit-details").Include(
                "~/Scripts/Biobank/details-logo.js",
                "~/Scripts/Biobank/edit-details.js"));
            bundles.Add(new ScriptBundle("~/bundles/collection-and-capability-crud").Include(
                        "~/Scripts/bootbox*",
                        "~/Scripts/typeahead.bundle*",
                        "~/Scripts/Biobank/associated-data-highlight.js",
                        "~/Scripts/Shared/diagnosis-type-ahead.js"));
            bundles.Add(new ScriptBundle("~/bundles/help-buttons").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/Biobank/help-popups.js"));
            bundles.Add(new ScriptBundle("~/bundles/collection-sample-sets").Include(
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/knockout.validation*",
                        "~/Scripts/bootstrap-slider.min.js",
                        "~/Scripts/bootstrap-slider-knockout-binding.js",
                        "~/Scripts/ko.bindingHandlers.dataTable.js",
                        "~/Scripts/Biobank/collection-sample-sets.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/collections").Include(
                "~/Scripts/Biobank/collections.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/capabilities").Include(
                "~/Scripts/Biobank/capabilities.js"));

            bundles.Add(new ScriptBundle("~/bundles/biobank/publications").Include(
                "~/Scripts/Biobank/publications.js"));

            bundles.Add(new ScriptBundle("~/bundles/biobank/collection").Include(
                "~/Scripts/Biobank/collection.js",
                "~/Scripts/Shared/associated-data-admin.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/capability").Include(
                "~/Scripts/Shared/associated-data-admin.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/sampleset").Include(
                "~/Scripts/Biobank/sampleset.js"));
            bundles.Add(new ScriptBundle("~/bundles/biobank/networkaccept").Include(
                "~/Scripts/Biobank/network-acceptance.js"));

            //Account
            bundles.Add(new ScriptBundle("~/bundles/reset-password").Include(
                    "~/Scripts/jquery.pwstrength.bootstrap/pwstrength-bootstrap*",
                    "~/Scripts/Account/reset-password.js"));

            //Profile
            bundles.Add(new ScriptBundle("~/bundles/profile/biobank").Include(
                "~/Scripts/Profile/biobank.js"));

            bundles.Add(new ScriptBundle("~/bundles/profile/publications").Include(
                "~/Scripts/Profile/publications.js"));

            //Network Admin
            bundles.Add(new ScriptBundle("~/bundles/network/admins").Include(
                "~/Scripts/Shared/registerentity-admin-viewmodel.js",
                "~/Scripts/Network/admins-viewmodel.js"));
            bundles.Add(new ScriptBundle("~/bundles/network/edit-details").Include(
                "~/Scripts/Network/details-logo.js"));
            bundles.Add(new ScriptBundle("~/bundles/network/add-biobank").Include(
                "~/Scripts/typeahead.bundle*",
                "~/Scripts/Network/biobanks-type-ahead.js"));
            bundles.Add(new ScriptBundle("~/bundles/network/biobanks").Include(
                "~/Scripts/Network/network-biobanks.js"));

            //ADAC Admin
            bundles.Add(new ScriptBundle("~/bundles/adac/disease-statuses").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-disease-statuses.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/material-types").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-material-types.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/sexes").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-sexes.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/country").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-country.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/county").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-county.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/associated-data-types").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-associated-data-types.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/associated-data-type-groups").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-associated-data-type-group.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/registration-reasons").Include(
                 "~/Scripts/bootbox*",
                 "~/Scripts/ADAC/adac-registration-reasons.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/collection-percentages").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-collection-percentages.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/collection-points").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-collection-points.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/sample-collection-modes").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-sample-collection-modes.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/collection-type").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-collection-type.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/consent-restriction").Include(
               "~/Scripts/bootbox*",
               "~/Scripts/ADAC/adac-consent-restriction.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/hta-status").Include(
               "~/Scripts/bootbox*",
               "~/Scripts/ADAC/adac-hta-status.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/collection-status").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-collection-status.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/associated-data-procurement-time-frame").Include(
               "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-associated-data-procurement-time-frame.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/service-offering").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-service-offering.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/macro-assessments").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-macro-assessments.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/donor-counts").Include(
               "~/Scripts/bootbox*",
               "~/Scripts/ADAC/adac-donor-counts.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/storage-temperatures").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-storage-temperatures.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/sop-status").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-sop-status.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/age-ranges").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-age-ranges.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/access-conditions").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-access-conditions.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/biobankactivity").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-biobankactivity.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/biobanks").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-biobanks.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/deletebiobank").Include(
                "~/Scripts/ADAC/adac-deletebiobank.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/funders").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-funders.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/deletefunder").Include(
                "~/Scripts/ADAC/adac-deletefunder.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/networks").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-networks.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/requests").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-requests.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/historical").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-historical.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/annual-statistics").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-annual-statistics.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));
            bundles.Add(new ScriptBundle("~/bundles/adac/annual-statistic-group").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-annual-statistic-group.js",
                "~/Scripts/ADAC/adac-refdata-utility.js"));

            bundles.Add(new ScriptBundle("~/bundles/adac/tabs").Include(
                "~/Scripts/ADAC/adac-tabs.js"));

            // Site Config
            bundles.Add(new ScriptBundle("~/bundles/adac/site-config").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-site-config.js"));

            // Sample Resource Config
            bundles.Add(new ScriptBundle("~/bundles/adac/sample-resource-config").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-sample-resource-config.js"));

            // Register Pages Config
            bundles.Add(new ScriptBundle("~/bundles/adac/register-pages-config").Include(
                "~/Scripts/bootbox*",
                "~/Scripts/ADAC/adac-register-pages-config.js"));

            // Search
            bundles.Add(new ScriptBundle("~/bundles/search").Include(
                "~/Scripts/typeahead.bundle*",
                "~/Scripts/Shared/search-diagnosis-type-ahead.js",
                "~/Scripts/Search/facet-groups.js"));

            // Shared / Layout
            bundles.Add(new ScriptBundle("~/bundles/shared").Include(
                "~/Scripts/Contact/contact-button.js",
                "~/Scripts/Shared/feedback-message.js"));

            //Term 
            bundles.Add(new ScriptBundle("~/bundles/term/diseasetable").Include(
               "~/Scripts/Term/diseasetable.js"));

            //Analytics
            bundles.Add(new ScriptBundle("~/bundles/analytics/biobankreport").Include(
                "~/Scripts/Biobank/analytics-biobankreport.js",
                "~/Scripts/plotly-latest.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/analytics/directoryreport").Include(
                "~/Scripts/ADAC/adac-analytics-directoryreport.js",
                "~/Scripts/plotly-latest.min.js"));
            #endregion
        }
    }

}
