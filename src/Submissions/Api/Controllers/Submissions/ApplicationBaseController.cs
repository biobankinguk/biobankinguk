using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Submissions.Api.Controllers.Submissions
{
    public class ApplicationBaseController : Controller
    {
        // TODO: Update or remove the user model.
        // public ApplicationUserPrincipal CurrentUser => User.ToApplicationUserPrincipal();
        
        public Dictionary<string, string> Config => GetSiteConfig();

        /// <summary>
        /// Gets the Page Strings config.
        /// </summary>
        /// <returns>A dictionary of the config keys and values.</returns>
        protected Dictionary<string, string> GetSiteConfig()
        {
            var config = PageStrings.DefaultConfigs;

            return config.ToDictionary(x => x.Key, x => x.Value);
        }

        protected void SetTemporaryFeedbackMessage(string message, FeedbackMessageType type, bool containsHtml = false)
            => TempData[ViewConstants.FeedbackMessageKey] = new FeedbackMessage
            {
                Message = message,
                Type = type,
                ContainsHtml = containsHtml
            };
        
        public static class ViewConstants
        {
            public const string FeedbackMessageKey = "TemporaryFeedbackMessage";
        }
        
    }
}