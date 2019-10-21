using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class CountyDto : RefDataBaseDto
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; } = null!;
    }
}
