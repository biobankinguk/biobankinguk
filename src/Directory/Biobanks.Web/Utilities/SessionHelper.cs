using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Biobanks.Web.Extensions;
using Newtonsoft.Json;
using RazorEngine.Templating;

namespace Biobanks.Web.Utilities
{
    public static class SessionHelper
    {
        public static int GetBiobankId(HttpSessionStateBase session)
        {
            var activeOrganisationType = Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]);
            var activeOrganisationId = Convert.ToInt32(session[SessionKeys.ActiveOrganisationId]);

            if (activeOrganisationType == (int) ActiveOrganisationType.Biobank
                || activeOrganisationType == (int) ActiveOrganisationType.NewBiobank)
            {
                return activeOrganisationId;
            }

            // if no biobankId in session, return 0
            return 0;
        }

        public static int GetNetworkId(HttpSessionStateBase session)
        { 
            var activeOrganisationType = Convert.ToInt32(session[SessionKeys.ActiveOrganisationType]);
            var activeOrganisationId = Convert.ToInt32(session[SessionKeys.ActiveOrganisationId]);

            if (activeOrganisationType == (int) ActiveOrganisationType.Network
                || activeOrganisationType == (int) ActiveOrganisationType.NewNetwork)
            {
                return activeOrganisationId;
            }

            // if no networkId in session, return 0
            return 0;
        }
    }
}