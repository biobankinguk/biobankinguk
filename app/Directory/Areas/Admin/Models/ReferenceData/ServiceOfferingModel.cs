using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;

namespace Biobanks.Directory.Areas.Admin.Models.ReferenceData;

public class ServiceOfferingModel
{
  public ICollection<ReadServiceOfferingModel> ServiceOfferings;
}
