using System.Collections.Generic;

namespace Biobanks.Directory.Areas.Admin.Models.Requests;

public class RequestsModel
{
  public ICollection<BiobankRequestModel> BiobankRequests { get; set; }
  public ICollection<NetworkRequestModel> NetworkRequests { get; set; }
}
