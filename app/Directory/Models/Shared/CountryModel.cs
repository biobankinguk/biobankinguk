using System.ComponentModel.DataAnnotations;

namespace Biobanks.Directory.Models.Shared
{
    public class CountryModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}

