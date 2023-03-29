using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Areas.Network.Models.Profile;

public class AddBiobankToNetworkModel
{
  [Display(Name = "Resource Name")]
  public string BiobankName { get; set; }

  [Display(Name = "Your ID")]
  public string BiobankExternalID { get; set; }
}
