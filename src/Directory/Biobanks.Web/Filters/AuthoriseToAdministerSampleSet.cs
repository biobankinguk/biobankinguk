using System;
using System.Web.Mvc;
using Directory.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Web.Utilities;
using Biobanks.Web.Results;

namespace Biobanks.Web.Filters
{
    public class AuthoriseToAdministerSampleSet : System.Web.Mvc.AuthorizeAttribute
    {
        public IBiobankReadService BiobankReadService { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var session = filterContext.RequestContext.HttpContext.Session;
            var activeOrganisationId = (int) session[SessionKeys.ActiveOrganisationId];

            if (Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.Biobank
                && filterContext.RequestContext.HttpContext.User.ToApplicationUserPrincipal()
                .Biobanks.ContainsKey(activeOrganisationId))
            {
                var sampleSetId = int.Parse(filterContext.Controller.ValueProvider.GetValue("id").AttemptedValue);

                if (BiobankReadService.CanThisBiobankAdministerThisSampleSet(activeOrganisationId, sampleSetId))
                {
                    return;
                }
            }

            filterContext.Result = new HttpForbiddenResult();
        }
    }
}
