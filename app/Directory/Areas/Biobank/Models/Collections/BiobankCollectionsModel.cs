using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Biobank.Models.Collections;

public class BiobankCollectionsModel
{
  public IEnumerable<BiobankCollectionModel> BiobankCollectionModels { get; set; }
}

public class BiobankCollectionModel
{
  public int Id { get; set; }
  public string OntologyTerm { get; set; }
  public string Title { get; set; }
  public int StartYear { get; set; }
  public string MaterialTypes { get; set; }
  public int NumberOfSampleSets { get; set; }
}
