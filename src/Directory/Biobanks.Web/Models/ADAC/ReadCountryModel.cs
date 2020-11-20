using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.ADAC
{
    public class ReadCountryModel : Shared.CountryModel
    {
        //Sum of all Counties and Organisations
        public int CountyOrganisationCount { get; set; }
    }
}