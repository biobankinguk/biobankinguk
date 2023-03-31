namespace Biobanks.Directory.Areas.Admin.Models.Requests;

public class BiobankRequestModel
{
  public int RequestId { get; set; }
  public string BiobankName { get; set; }
  public string UserName { get; set; }
  public string UserEmail { get; set; }
}
