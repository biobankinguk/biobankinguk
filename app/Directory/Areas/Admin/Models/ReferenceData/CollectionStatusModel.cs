using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Areas.Admin.Models.ReferenceData;

public class CollectionStatusModel
{
  public ICollection<ReadCollectionStatusModel> CollectionStatuses;
}
