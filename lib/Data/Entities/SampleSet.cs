using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class SampleSet
    {
        [Key]
        public int Id { get; set; } //"Sample Code" in model

        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }

        public int SexId { get; set; }
        public virtual Sex Sex { get; set; }

        public int AgeRangeId { get; set; }
        public virtual AgeRange AgeRange { get; set; }

        public int DonorCountId { get; set; }
        public virtual DonorCount DonorCount { get; set; }

        public virtual ICollection<MaterialDetail> MaterialDetails { get; set; }
    }
}
