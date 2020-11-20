using System.Configuration;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using Biobanks.Web.Results;

namespace Biobanks.Web.Filters
{
    public class UserAuthenticateAttribute : FilterAttribute, IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //Skip Authentication for AllowAnonymous decorated contexts (Action or Controller, with inheritance)
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute),
                    inherit: true))
                return;

            //Check that the user is authenticated
            if (!filterContext.Principal.Identity.IsAuthenticated)
                filterContext.Result = new HttpUnauthorizedResult(); //401 Unauthorised
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //Auth Challenge always occurs in Authenticate/Authorise decorated scenarios
            //even if successful
            //It essentially interrupts the flow on failure,
            //or otherwise takes place after the Action method but before the result
            //https://www-asp.azureedge.net/v-2015-12-16-016/media/4773381/lifecycle-of-an-aspnet-mvc-5-application.pdf

            //Handle 401 Unauthorised results (require a login)
            //No need to handle 401 ourselves (though we could if we wanted to)
            //ASP.NET Identity automagically converts a 401 into a 302 Redirect
            //to the configured LoginPath, with ReturnUrlParameter included

            //Handle 403 Forbidden (redirect to an error page)
            if (filterContext.Result is HttpForbiddenResult)
            {
                //ASP.NET Identity doesn't automagically do this one!

                //So we need to change the 403 result to a redirect
                //Return Url is unnecessary here
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"action", ConfigurationManager.AppSettings["ForbiddenActionMethod"]},
                        {"controller", ConfigurationManager.AppSettings["ForbiddenController"]}
                    });
            }

            //Anything else?
        }
    }
}
