using System;

namespace Biobanks.Directory.Areas.Admin.Models.Requests;

public class HistoricalRequestModel
{
  public string UserName { get; set; }
  public string UserEmail { get; set; }

  public string EntityName { get; set; }
  public string Action { get; set; }
  public DateTime Date { get; set; }

  public bool UserEmailConfirmed { get; set; }

  // the biobank or network created from finishing the registration
  public string ResultingOrgExternalId { get; set; }
  
}
