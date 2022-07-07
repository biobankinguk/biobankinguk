using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class SampleSet
    {
        [Key]
        public int Id { get; set; } //"Sample Code" in model

        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; }

        public int SexId { get; set; }
        public virtual Sex Sex { get; set; }

        public virtual ICollection<AgeRange> AgeRanges { get; set; }

        //TODO migrate data and then delete this column
        public int AgeRangeId { get; set; }

        [NotMapped]
        public virtual AgeRange AgeRange { get; set; }

        public int DonorCountId { get; set; }
        public virtual DonorCount DonorCount { get; set; }

        public virtual ICollection<MaterialDetail> MaterialDetails { get; set; }
    }
}
