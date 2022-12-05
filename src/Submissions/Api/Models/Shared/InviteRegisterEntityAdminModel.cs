using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared;

public class InviteRegisterEntityAdminModel
{
  [Required(ErrorMessage = "Please enter the name of the person you're inviting.")]
  public string Name { get; set; }

  [Required(ErrorMessage = "Please enter an email address to send an invitation to.")]
  [DataType(DataType.EmailAddress)]
  [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
  public string Email { get; set; }


  public string EntityName { get; set; } //for label
  [Required]
  public string Entity { get; set; }
  /// <summary>
  /// The controller to which this will be redirected (either the biobank or ADAC depending on who is adding admins)
  /// </summary>

  ///TODO: Fix once JS urls are done 
  public string ControllerName { get; set; }
}
