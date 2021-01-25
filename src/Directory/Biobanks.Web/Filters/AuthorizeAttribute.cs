using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Directory.Services.Contracts;
using Biobanks.Web.Extensions;
using Biobanks.Web.Results;
using Biobanks.Web.Utilities;

namespace Biobanks
{
    public class AuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public IBiobankReadService BiobankReadService { get; set; }

        private string _claimType;

        public string ClaimType
        {
            get => _claimType ?? string.Empty;
            set => _claimType = value;
        }

        public bool AllowSuspendedBiobanks { get; set; } = true;

        private BiobanksAuthorizeFailure _failureType = BiobanksAuthorizeFailure.None;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var user = httpContext.User;
            var currentUser = user.ToApplicationUserPrincipal();

            if (!user.Identity.IsAuthenticated)
            {
                return false;
            }

            //Check ClaimType if one is specified
            if (!string.IsNullOrEmpty(ClaimType))
            {
                if (currentUser.Claims.All(x => x.Type != ClaimType))
                {
                    _failureType = BiobanksAuthorizeFailure.MissingClaimType;
                    return false;
                }
            }

            // verify network claim
            var session = httpContext.Session;
            var activeOrganisationId = Convert.ToInt32(session[SessionKeys.ActiveOrganisationId]);
            var activeOrganisationType = Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]);


            if (activeOrganisationId != 0)
            {
                if (activeOrganisationType == (int)ActiveOrganisationType.Biobank)
                {
                    // If they don't have a claim on this biobank, return
                    if (!currentUser.Biobanks.ContainsKey(activeOrganisationId))
                    {
                        _failureType = BiobanksAuthorizeFailure.Unauthorized;
                        return false;
                    }

                    //Check for Suspended Biobanks if they are disallowed
                    if (AllowSuspendedBiobanks) return true;

                    var bb =
                        Task.Run(async () =>
                                await BiobankReadService.GetBiobankByIdAsync(activeOrganisationId))
                            .Result;

                    //only fail if suspended
                    if (bb != null && bb.IsSuspended)
                    {
                        _failureType = BiobanksAuthorizeFailure.BiobankSuspended;
                        return false;
                    }
                }

                else if (activeOrganisationType == (int) ActiveOrganisationType.Network)
                {

                    // If they don't have a claim on this biobank, return
                    if (!currentUser.Networks.ContainsKey(activeOrganisationId))
                    {
                        _failureType = BiobanksAuthorizeFailure.Unauthorized;
                        return false;
                    }
                }
            }
            else
            {
                // no biobankId
                _failureType = BiobanksAuthorizeFailure.Unauthorized;
                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            switch (_failureType)
            {
                case BiobanksAuthorizeFailure.Unauthorized:
                    filterContext.Result = new UnauthorizedResult();
                    break;
                case BiobanksAuthorizeFailure.MissingClaimType:
                    filterContext.Result = new MissingClaimTypeResult(ClaimType);
                    break;
                case BiobanksAuthorizeFailure.BiobankSuspended:
                    var session = filterContext.HttpContext.Session;
                    var activeOrganisationId = Convert.ToInt32(session[SessionKeys.ActiveOrganisationId]);
                    var activeOrganisationType = Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]);

                    if (activeOrganisationId != 0 && activeOrganisationType ==
                        (int)ActiveOrganisationType.Biobank)
                    {
                        var bb =
                            Task.Run(async () =>
                                    await BiobankReadService.GetBiobankByIdAsync(activeOrganisationId))
                                .Result;
                        filterContext.Result = new BiobankSuspendedResult(bb.Name);
                    }

                    break;
            }
        }
    }

    internal enum BiobanksAuthorizeFailure
    {
        None,
        MissingClaimType,
        BiobankSuspended,
        Unauthorized
    }
}
