using Biobanks.Entities.Shared.ReferenceData;
using System;
using System.Collections.Generic;
using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class Capability
    {
        public int CapabilityId { get; set; }

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
