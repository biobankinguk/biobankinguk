using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class SopStatusesModel
    {
        public ICollection<SopStatusModel> SopStatuses { get; set; }
    }

    public class SopStatusModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}

