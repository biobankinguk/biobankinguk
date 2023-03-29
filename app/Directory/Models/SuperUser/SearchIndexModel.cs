namespace Biobanks.Submissions.Api.Models.SuperUser;

public class SearchIndexModel
{
  public int TotalSampleSetCount { get; set; }
  public int IndexableSampleSetCount { get; set; }
  public int SuspendedSampleSetCount { get; set; }
  public long CollectionSearchDocumentCount { get; set; }
  public int TotalCapabilityCount { get; set; }
  public int IndexableCapabilityCount { get; set; }
  public int SuspendedCapabilityCount { get; set; }
  public long CapabilitySearchDocumentCount { get; set; }
}
