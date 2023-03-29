using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Search;

public class DetailedCapabilitySearchCapabilityModel
{
    public string Disease { get; set; }
    public string Protocols { get; set; }
    public string AnnualDonorExpectation { get; set; }
    public IEnumerable<KeyValuePair<string, string>> AssociatedData { get; set; }
}
