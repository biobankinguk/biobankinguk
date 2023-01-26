namespace Biobanks.Submissions.Api.Areas.Admin.Models.Requests;

public class NetworkRequestModel
{
  public int RequestId { get; set; }
  public string NetworkName { get; set; }
  public string UserName { get; set; }
  public string UserEmail { get; set; }
}
