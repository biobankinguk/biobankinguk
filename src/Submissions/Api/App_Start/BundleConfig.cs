using System.Web.Optimization;

namespace Biobanks.Submissions.Api.App_Start
{
    public static class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/pagesadmin").Include(
                "~/Scripts/Directory/directory-pages-admin.js",
                "~/Scripts/bootbox*",
                "~/Scripts/Directory/directory-refdata-utility.js"));
        }
    }
}
