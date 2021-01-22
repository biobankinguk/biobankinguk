using System;
using System.Linq;
using System.Web.Mvc;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Web.Utilities;
using Biobanks.Web.Results;

namespace Biobanks.Web.Filters
{
    public class AuthoriseToAdministerCollection : System.Web.Mvc.AuthorizeAttribute
    {
        public IBiobankReadService BiobankReadService { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            var session = filterContext.RequestContext.HttpContext.Session;
            var activeOrganisationId = session[SessionKeys.ActiveOrganisationId];

            if (activeOrganisationId != null
                && Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.Biobank
                && filterContext.RequestContext.HttpContext.User.ToApplicationUserPrincipal().BiobankIds
                    .Contains(activeOrganisationId.ToString()))
            {
                var biobankId = (int) activeOrganisationId;
                var collectionId = int.Parse(filterContext.Controller.ValueProvider.GetValue("id").AttemptedValue);

                if (BiobankReadService.CanThisBiobankAdministerThisCollection(biobankId, collectionId))
                {
                    return;
                }
            }

            filterContext.Result = new HttpForbiddenResult();
        }
    }
}
