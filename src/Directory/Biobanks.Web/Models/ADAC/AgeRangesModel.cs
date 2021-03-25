﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class AgeRangesModel
    {
        public ICollection<AgeRangeModel> AgeRanges { get; set; }
    }

    public class AgeRangeModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }

        public string LowerBound { get; set; }
        public string UpperBound { get; set; }
    }
}