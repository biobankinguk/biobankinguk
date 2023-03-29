using System;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Settings;

public class NetworkAcceptanceModel
{
  public int BiobankId { get; set; }
  public int NetworkId {get; set; }
  public string NetworkName { get; set; }
  public string NetworkDescription { get; set; }
  public string NetworkEmail { get; set; }
  public DateTime? ApprovedDate { get; set; }
}
