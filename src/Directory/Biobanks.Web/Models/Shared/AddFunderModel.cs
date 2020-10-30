using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class AddFunderModel
    {
        [DisplayName("Funder Name")]
        [Required]
        public string FunderName { get; set; }

        public string BiobankName { get; set; }
    }
}