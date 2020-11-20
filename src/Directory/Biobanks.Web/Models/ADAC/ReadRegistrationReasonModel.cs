using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadRegistrationReasonModel : Shared.RegistrationReasonModel
    {
        public int OrganisationCount { get; set; }
    }
}