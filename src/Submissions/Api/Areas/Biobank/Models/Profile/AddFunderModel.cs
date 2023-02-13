using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Areas.Biobank.Models.Profile;

public class AddFunderModel
{
    [DisplayName("Funder Name")]
    [Required]
    public string FunderName { get; set; }

    public string BiobankName { get; set; }
    
    public string BiobankId { get; set; }
}
