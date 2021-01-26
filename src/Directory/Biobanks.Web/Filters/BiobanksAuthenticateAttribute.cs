using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using Directory.Identity.Constants;
using Biobanks.Web.Results;

namespace Biobanks.Web.Filters
{
    public class BiobanksAuthenticateAttribute : IAuthenticationFilter
    {
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            //Nothing special to do here
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            //Handle Custom Biobanks Authorisation failures

            //An expected claim type is missing from the user principal
            if (filterContext.Result is MissingClaimTypeResult)
            {
                //which claim type? is it one we handle when missing?
                switch (((MissingClaimTypeResult)filterContext.Result).ExpectedClaimType)
                {
                    case CustomClaimType.Biobank:
                        //We were sufficiently authorised to have BiobankAdmin Role
                        //But no BiobankId claim: Biobank Details need completing!
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                {"action", "Edit" },
                                {"controller", "Biobank" },
                                {"detailsIncomplete", "true" }
                            });
                        break;

                    case CustomClaimType.Network:
                        //We were sufficiently authorised to have NetworkAdmin Role
                        //But no NetworkId claim: Network Details need completing!
                        filterContext.Result = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                {"action", "Edit" },
                                {"controller", "Network" },
                                {"detailsIncomplete", "true" }
                            });
                        break;

                    default:
                        //we don't know how to handle it, chuck a 403 Forbidden (since this is a failed authorisation result)
                        filterContext.Result = new HttpForbiddenResult();
                        break;
                }
            }

            //Biobank is suspended
            if (filterContext.Result is BiobankSuspendedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"action", "Suspended"},
                        {"controller", "Biobank"},
                        {"biobankName", ((BiobankSuspendedResult) filterContext.Result).BiobankName}
                    });
            }

            if (filterContext.Result is UnauthorizedResult)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {"action", "Forbidden"},
                        {"controller", "Account"},
                    });
            }
        }
    }
}
