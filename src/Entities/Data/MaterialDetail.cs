using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Entities.Data.ReferenceData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biobanks.Entities.Data
{
    public class MaterialDetail //"Material Preservation Details" in the model
    {
        [Key]
        public int Id { get; set; }

        public int SampleSetId { get; set; } //"Sample Code" in the model
        public virtual SampleSet SampleSet { get; set; }

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

