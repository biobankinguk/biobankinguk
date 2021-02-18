using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;

namespace Biobanks.Web.Filters
{
    public class UserApiAuthorizeAttribute: System.Web.Http.AuthorizeAttribute
    {
        //override the HTTP Unauthorized handler to return a 403 Forbidden, instead of 401 Unauthorized
        protected override void HandleUnauthorizedRequest(HttpActionContext filterContext)
        {
            filterContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden);
        }
    }
}