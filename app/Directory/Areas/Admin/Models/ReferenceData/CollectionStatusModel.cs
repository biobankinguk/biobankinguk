using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.ReferenceData;

public class CollectionStatusModel
{
  public ICollection<ReadCollectionStatusModel> CollectionStatuses;
}
