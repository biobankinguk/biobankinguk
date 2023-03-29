using System.Collections.Generic;

namespace Biobanks.Directory.Models.Shared;

public class AssociatedDataGroupModel
{
  public int GroupId { get; set; }
  public string Name { get; set; }
  public List<AssociatedDataModel> Types { get; set; }
}
