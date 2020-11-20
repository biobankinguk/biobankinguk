using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Network
{
    public class AddBiobankToNetworkModel
    {
        [Display(Name = "Resource Name")]
        public string BiobankName { get; set; }

        [Display(Name = "Your ID")]
        public string BiobankExternalID { get; set; }
    }
}