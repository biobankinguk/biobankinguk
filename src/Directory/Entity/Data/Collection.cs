using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class Collection
    {
        public int CollectionId { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public int DiagnosisId { get; set; }
        public virtual Diagnosis Diagnosis { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }

        public string Description { get; set; }

        ///[DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool FromApi { get; set; }

        public int? HtaStatusId { get; set; }
        public virtual HtaStatus HtaStatus { get; set; }

        public int AccessConditionId { get; set; }
        public virtual AccessCondition AccessCondition { get; set; }

        public int? CollectionTypeId { get; set; }
        public virtual CollectionType CollectionType { get; set; }

        public int CollectionStatusId { get; set; }
        public virtual CollectionStatus CollectionStatus { get; set; }

        public int CollectionPointId { get; set; }
        public virtual CollectionPoint CollectionPoint { get; set; }

        public virtual ICollection<CollectionSampleSet> SampleSets { get; set; }

        public virtual ICollection<CollectionAssociatedData> AssociatedData { get; set; }

        public virtual ICollection<ConsentRestriction> ConsentRestrictions { get; set; }
    }
}
