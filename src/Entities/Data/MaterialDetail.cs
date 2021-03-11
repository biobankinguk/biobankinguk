using Biobanks.Entities.Shared.ReferenceData;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Biobanks.Entities.Data.ReferenceData;

namespace Biobanks.Entities.Data
{
    public class MaterialDetail //"Material Preservation Details" in the model
    {
        [Key, Column(Order = 0)]
        public int SampleSetId { get; set; } //"Sample Code" in the model

        [Key, Column(Order = 1)]
        public int MaterialTypeId { get; set; }
        public virtual MaterialType MaterialType { get; set; }

        [Key, Column(Order = 2)]
        public int StorageTemperatureId { get; set; }
        public virtual StorageTemperature StorageTemperature { get; set; }

        [Key, Column(Order = 3)]
        public int MacroscopicAssessmentId { get; set; }
        public virtual MacroscopicAssessment MacroscopicAssessment { get; set; }

        [Key, Column(Order = 4)]
        public int ExtractionProcedureId { get; set; }
        public virtual ExtractionProcedure ExtractionProcedure { get; set; }

        [Key, Column(Order = 5)]
        public int PreservationTypeId { get; set; }
        public virtual PreservationType PreservationType { get; set; }

        public int? CollectionPercentageId { get; set; }
        public virtual CollectionPercentage CollectionPercentage { get; set; }
    }
}

