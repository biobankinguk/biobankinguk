using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Services.Contracts;
using Biobanks.Web.Controllers;
using Biobanks.Web.Utilities;

namespace Biobanks.Web.Filters
{
    public class SuspendedWarningAttribute : FilterAttribute, IActionFilter
    {
        public IOrganisationService OrganisationService { get; set; }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //Technically this shouldn't be an issue, since we know where we're using this,
            //But let's be safe and make sure that the controller using this filter
            //is derived from ApplicationBaseController
            var controller = filterContext.Controller;
            var type = controller.GetType();
            if (controller.GetType().BaseType != typeof (ApplicationBaseController)) return;

            //Ok, now we can safely cast, and get biobank id from CurrentUser :)
            var biobankId = SessionHelper.GetBiobankId(filterContext.HttpContext.Session);

            //Check if suspended
            controller.ViewBag.ShowSuspendedWarning =
                Task.Run(
                    async () =>
                        await OrganisationService.IsBiobankSuspendedAsync(biobankId))
                    .Result;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {   }
    }
}
