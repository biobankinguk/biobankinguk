using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using RazorEngine.Templating;

namespace Biobanks.Web.Utilities
{
    public static class SessionHelper
    {
        public static int GetBiobankId(HttpSessionStateBase session)
        {
            if (Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.Biobank
                || Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.NewBiobank)
            {
                return (int) session[SessionKeys.ActiveOrganisationId];
            }

            // if no biobankId in session, return 0
            return 0;
        }

        public static int GetNetworkId(HttpSessionStateBase session)
        {
            if (Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.Network
                || Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]) == (int) ActiveOrganisationType.NewNetwork)
            {
                return (int) session[SessionKeys.ActiveOrganisationId];
            }

            // if no networkId in session, return 0
            return 0;
        }
    }
}