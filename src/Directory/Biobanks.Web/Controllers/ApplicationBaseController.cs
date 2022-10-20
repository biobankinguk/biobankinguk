using System.Web.Mvc;
using Biobanks.Identity;
using Biobanks.Web.Extensions;
using Biobanks.Web.Utilities;
using System.Collections.Generic;
using Biobanks.Web.Models.Shared;
using System;

namespace Biobanks.Web.Controllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
    public abstract class ApplicationBaseController : Controller
    {
        public ApplicationUserPrincipal CurrentUser => User.ToApplicationUserPrincipal();

        public Dictionary<string, string> Config => GetSiteConfig();

        protected Dictionary<string, string> GetSiteConfig()
        {
            var config = HttpContext.Application["Config"];
            
            return (config as Dictionary<string, string>) ?? new Dictionary<string, string>();
        }

        protected void SetTemporaryFeedbackMessage(string message, FeedbackMessageType type, bool containsHtml = false)
            => TempData[ViewConstants.FeedbackMessageKey] = new FeedbackMessage
            {
                Message = message,
                Type = type,
                ContainsHtml = containsHtml
            };

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled) return;

            //Handle AntiForgeryToken errors differently to the generic custom error
            if (filterContext.Exception is HttpAntiForgeryException)
            {
                //redirect to the "You submitted the form twice!" error page
                //any routes which require a non-standard handler for HttpAntiForgeryException
                //should have it configured in their controller's OnError()
                filterContext.Result = RedirectToRoute(
                    new
                    {
                        controller = "Error",
                        action = "CSRFToken"
                    });
                filterContext.ExceptionHandled = true;
            }

            //If we've got here and it's still not handled, it should be (and HandleError's result will apply)
            filterContext.ExceptionHandled = true;
        }

    }

    public static class ViewConstants
    {
        public const string FeedbackMessageKey = "TemporaryFeedbackMessage";
    }

}
