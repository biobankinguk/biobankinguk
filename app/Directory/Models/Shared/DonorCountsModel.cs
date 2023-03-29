using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class DonorCountsModel
    {
        public ICollection<DonorCountModel> DonorCounts { get; set; }
    }

    public class DonorCountModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int? LowerBound { get; set; }

        public int? UpperBound { get; set; }

        public int SampleSetsCount { get; set; }
    }
}

