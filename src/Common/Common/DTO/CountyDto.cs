using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class CountyDto
    {
        public int CountryId { get; set; }
        [Required]
        public string CountryName { get; set; } = null!;
    }
}
