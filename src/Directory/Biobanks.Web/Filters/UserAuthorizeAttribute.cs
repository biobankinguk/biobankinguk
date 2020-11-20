using Biobanks.Web.Results;
using System.Web.Mvc;

namespace Biobanks.Web.Filters
{
    public class UserAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        //override the MVC Unauthorized handler to return a 403 Forbidden, instead of 401 Unauthorized
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new HttpForbiddenResult();
        }
    }
}
