using System.Collections.Generic;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AgeRange : BaseReferenceData
    {
        public string LowerBound { get; set; }

        public string UpperBound { get; set; }

        public virtual ICollection<SampleSet> SampleSets { get; set; }
    }
}
