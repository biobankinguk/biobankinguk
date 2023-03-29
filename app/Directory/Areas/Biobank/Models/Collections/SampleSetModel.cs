using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Collections;

public class SampleSetModel
{
  public int Id { get; set; }
  public int CollectionId { get; set; }
  public string Sex { get; set; }
  public string AgeRange { get; set; }
  public string DonorCount { get; set; }

  public IEnumerable<MaterialPreservationDetailModel> MaterialPreservationDetails { get; set; }

  public bool ShowMacroscopicAssessment { get; set; }
}
public class MaterialPreservationDetailModel
{
  public string CollectionPercentage { get; set; }
  public string MacroscopicAssessment { get; set; }
  public string MaterialType { get; set; }
  public string PreservationType { get; set; }
  public string StorageTemperature { get; set; }
  public string ExtractionProcedure { get; set; }
}
