using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Areas.Admin.Models.Funders;

public class FunderModel
{
  public int FunderId { get; set; }

  [Required]
  public string Name { get; set; }
}
