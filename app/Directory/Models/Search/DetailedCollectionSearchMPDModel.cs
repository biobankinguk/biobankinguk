namespace Biobanks.Submissions.Api.Models.Search;

public class DetailedCollectionSearchMPDModel
{
    public string MaterialType { get; set; }
    public string StorageTemperature { get; set; }
    public string MacroscopicAssessment { get; set; }
    public string PercentageOfSampleSet { get; set; }
    public string PreservationType { get; set; }
    public string ExtractionProcedure { get; set; }
}
