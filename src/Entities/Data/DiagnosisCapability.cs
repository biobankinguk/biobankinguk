using System;
using System.Collections.Generic;

namespace Entities.Data
{
    public class DiagnosisCapability
    {
        public int DiagnosisCapabilityId { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public int DiagnosisId { get; set; }
        public virtual Diagnosis Diagnosis { get; set; }

        public DateTime LastUpdated { get; set; }

        public int SampleCollectionModeId { get; set; }
        public virtual SampleCollectionMode SampleCollectionMode { get; set; }

        public int AnnualDonorExpectation { get; set; }

        public virtual ICollection<CapabilityAssociatedData> AssociatedData { get; set; }
    }
}
