using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Data.Entities
{
    public class Collection
    {
        public int CollectionId { get; set; }

        public int OrganisationId { get; set; }
        public virtual Organisation Organisation { get; set; }

        public string OntologyTermId { get; set; }
        public virtual OntologyTerm OntologyTerm { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }

        public string Description { get; set; }

        ///[DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool FromApi { get; set; }

        // TODO: Remove the nullable context when the whole project supports .net 5+.
        #nullable enable
        public string? Notes { get; set; }
        #nullable disable

        public int AccessConditionId { get; set; }
        public virtual AccessCondition AccessCondition { get; set; }

        public int? CollectionTypeId { get; set; }
        public virtual CollectionType CollectionType { get; set; }

        public int CollectionStatusId { get; set; }
        public virtual CollectionStatus CollectionStatus { get; set; }

        public virtual ICollection<SampleSet> SampleSets { get; set; }

        public virtual ICollection<CollectionAssociatedData> AssociatedData { get; set; }

        public virtual ICollection<ConsentRestriction> ConsentRestrictions { get; set; }
    }
}
