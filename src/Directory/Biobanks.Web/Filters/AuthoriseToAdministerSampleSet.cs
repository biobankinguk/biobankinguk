using System;
using System.Linq;
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
            var activeOrganisationId = session[SessionKeys.ActiveOrganisationId];

            if (activeOrganisationId != null
                && Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.Biobank
                && filterContext.RequestContext.HttpContext.User.ToApplicationUserPrincipal()
                .Biobanks.FirstOrDefault(x => x.Key == Convert.ToInt32(activeOrganisationId)).Key != 0)
            {
                var biobankId = (int) activeOrganisationId;
                var sampleSetId = int.Parse(filterContext.Controller.ValueProvider.GetValue("id").AttemptedValue);

                if (BiobankReadService.CanThisBiobankAdministerThisSampleSet(biobankId, sampleSetId))
                {
                    return;
                }
            }

            filterContext.Result = new HttpForbiddenResult();
        }
    }
}
