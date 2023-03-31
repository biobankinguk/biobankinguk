using System;
using System.Collections.Generic;
using System.Globalization;

namespace Biobanks.Directory.Models.Shared;

public class AssociatedDataModel
{
  public bool Active { get; set; }
  public int? ProvisionTimeId { get; set; }
  public IEnumerable<AssociatedDataTimeFrameModel> TimeFrames { get; set; }
  public int DataTypeId { get; set; }
  public int DataGroupId { get; set; }
  public string DataTypeDescription { get; set; }
  public string Message { get; set; }

  public bool IsValid() => !Active || (Active && ProvisionTimeId != null && ProvisionTimeId > 0);
  public string LowercaseEntityName => DataTypeDescription.ToLower(CultureInfo.CurrentCulture).Replace(" ", string.Empty).Replace("/", string.Empty);

  public Boolean isLinked { get; set; } = false;
}
