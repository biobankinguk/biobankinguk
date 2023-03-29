using System;
using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class DiagnosisCapability
    {
        public int DiagnosisCapabilityId { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public string OntologyTermId { get; set; }
        public virtual OntologyTerm OntologyTerm { get; set; }

        public DateTime LastUpdated { get; set; }

        public int SampleCollectionModeId { get; set; }
        public virtual SampleCollectionMode SampleCollectionMode { get; set; }

        public int AnnualDonorExpectation { get; set; }

        public virtual ICollection<CapabilityAssociatedData> AssociatedData { get; set; }
    }
}
