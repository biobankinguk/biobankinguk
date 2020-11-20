using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.Shared
{
    public class FunderModel
    {
        public int FunderId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}