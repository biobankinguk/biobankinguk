using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Areas.Admin.Models;

public class FunderModel
{
  public int FunderId { get; set; }

  [Required]
  public string Name { get; set; }
}