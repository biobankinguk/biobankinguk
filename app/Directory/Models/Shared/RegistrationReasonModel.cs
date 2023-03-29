using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class RegistrationReasonModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
    }
}

