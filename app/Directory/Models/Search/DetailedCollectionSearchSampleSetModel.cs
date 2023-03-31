using System.Collections.Generic;

namespace Biobanks.Directory.Models.Search;

public class DetailedCollectionSearchSampleSetModel
{
    public string Sex { get; set; }
    public string AgeRange { get; set; }
    public string DonorCount { get; set; }

    public IEnumerable<DetailedCollectionSearchMPDModel> MaterialPreservationDetails { get; set; }
}
