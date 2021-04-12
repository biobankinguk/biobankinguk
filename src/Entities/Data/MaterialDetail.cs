using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class MaterialDetail //"Material Preservation Details" in the model
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        [Key, Column(Order = 1)]
        public int SampleSetId { get; set; } //"Sample Code" in the model

        public int MaterialTypeId { get; set; }
        public virtual MaterialType MaterialType { get; set; }

        public int StorageTemperatureId { get; set; }
        public virtual StorageTemperature StorageTemperature { get; set; }

        public int MacroscopicAssessmentId { get; set; }
        public virtual MacroscopicAssessment MacroscopicAssessment { get; set; }

        public string ExtractionProcedureId { get; set; }
        public virtual OntologyTerm ExtractionProcedure { get; set; }

        public int? PreservationTypeId { get; set; }
        public virtual PreservationType PreservationType { get; set; }

        public int? CollectionPercentageId { get; set; }
        public virtual CollectionPercentage CollectionPercentage { get; set; }
    }
}

