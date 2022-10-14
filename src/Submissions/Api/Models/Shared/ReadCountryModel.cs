using System;
namespace Biobanks.Submissions.Api.Models.Shared
{
    public class ReadCountryModel : CountryModel
    {
        //Sum of all Counties and Organisations
        public int CountyOrganisationCount { get; set; }
    }
}

