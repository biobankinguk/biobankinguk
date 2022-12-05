using System;
using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Models.Network;

public class NetworkBiobankModel
{
  public int BiobankId { get; set; }

  public string Name { get; set; }

  public DateTime? ApprovedDate { get; set; }

  public ICollection<string> Admins { get; set; }
}
