using Nest;

namespace Directory.Search.Dto.Documents
{
    public class MaterialPreservationDetailDocument
    {
        [Keyword(Name = "materialType")]
        public string MaterialType { get; set; }

        [Keyword(Name = "preservationType")]
        public string StorageTemperature { get; set; }

        [Keyword(Name = "preservationTypeMetadata")]
        public string StorageTemperatureMetadata { get; set; }

        [Keyword(Name = "macroscopicAssessment")]
        public string MacroscopicAssessment { get; set; }

        [Keyword(Name = "percentageOfSampleSet")]
        public string PercentageOfSampleSet { get; set; }
    }
}
