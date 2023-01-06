namespace Biobanks.Submissions.Api.Areas.Network.Models;

public class RegisterEntityAdminModel
{
  public string UserId { get; set; }
  public string UserFullName { get; set; }
  public string UserEmail { get; set; }
  public bool EmailConfirmed { get; set; }
}
