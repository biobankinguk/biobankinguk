using System;
using System.Collections.Generic;
using Biobanks.Directory.Models.Biobank;

namespace Biobanks.Directory.Areas.Biobank.Models.Collections;

public class CollectionModel
{
  public int Id { get; set; }
  public string Title { get; set; }
  public string Description { get; set; }
  public string OntologyTerm { get; set; }
  public DateTime StartDate { get; set; }
  public string AccessCondition { get; set; }
  public string CollectionType { get; set; }
  public bool FromApi { get; set; }
  public string Notes { get; set; }

  public IEnumerable<AssociatedDataSummaryModel> AssociatedData { get; set; }

  public IEnumerable<CollectionSampleSetSummaryModel> SampleSets { get; set; }
}

public class CollectionSampleSetSummaryModel
{
  public int Id { get; set; }
  public string Sex { get; set; }
  public string Age { get; set; }
  public string MaterialTypes { get; set; }
  public string PreservationTypes { get; set; }
  public string StorageTemperatures { get; set; }
  public string ExtractionProcedures { get; set; }
}
