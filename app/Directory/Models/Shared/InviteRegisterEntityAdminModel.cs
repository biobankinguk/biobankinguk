using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared;

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
  /// The controller the form will be submitted to (either the biobank, network, or Admin depending on who is adding admins)
  /// </summary>
  public string ControllerName { get; set; }
  
  /// <summary>
  /// The controller the form will redirect to on success (biobank / network / admin)
  /// </summary>
  public string SuccessControllerName { get; set; }  
  
  /// <summary>
  /// The area controller the form will redirect to on success (biobank / network / admin)
  /// </summary>
  public string SuccessAreaName { get; set; }
  
  /// <summary>
  /// The organisation the form will redirect to on success.
  /// </summary>
  public int OrganisationId { get; set; }
}
