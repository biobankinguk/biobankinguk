using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;
using System.Web.Http;
using System.Linq;

namespace Biobanks.Web.Filters
{
    public class UserApiAuthorizeAttribute: System.Web.Http.AuthorizeAttribute
    {
        public override void OnAuthorization(HttpActionContext filterContext)
        {
            //Skip Authentication for AllowAnonymous decorated contexts (Action or Controller, with inheritance)
            if(filterContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any() ||
                filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any())
                return;

            //Check that the user is authenticated
            if (!filterContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized); //401 Unauthorised
                return;
            }

            //Check that the user is authorised
            if (!IsAuthorized(filterContext))
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden); //403 Forbidden

        }
    }
}