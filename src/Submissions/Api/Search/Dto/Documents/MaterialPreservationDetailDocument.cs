using Nest;

namespace Biobanks.Search.Dto.Documents
{
    public class MaterialPreservationDetailDocument
    {
        [Keyword(Name = "materialType")]
        public string MaterialType { get; set; }

        [Keyword(Name = "storageTemperature")]
        public string StorageTemperature { get; set; }

        [Keyword(Name = "storageTemperatureMetadata")]
        public string StorageTemperatureMetadata { get; set; }

        [Keyword(Name = "macroscopicAssessment")]
        public string MacroscopicAssessment { get; set; }

        [Keyword(Name = "percentageOfSampleSet")]
        public string PercentageOfSampleSet { get; set; }

        [Keyword(Name = "preservationType")]
        public string PreservationType { get; set; }

        [Keyword(Name = "extractionProcedure")]
        public string ExtractionProcedure { get; set; }
    }
}
