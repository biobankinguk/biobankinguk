using Biobanks.Directory.Data.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Biobanks.Web.Models.ADAC
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