using System.Collections.Generic;

namespace Biobanks.Submissions.Api.Areas.Admin.Models;

public class RequestsModel
{
  public ICollection<BiobankRequestModel> BiobankRequests { get; set; }
  public ICollection<NetworkRequestModel> NetworkRequests { get; set; }
}
