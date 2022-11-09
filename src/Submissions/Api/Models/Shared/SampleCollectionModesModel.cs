using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class SampleCollectionModesModel
    {
        public ICollection<SampleCollectionModeModel> SampleCollectionModes { get; set; }
    }

    public class SampleCollectionModeModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}

