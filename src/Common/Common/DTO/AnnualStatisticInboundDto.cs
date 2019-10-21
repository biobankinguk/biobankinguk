using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class AnnualStatisticInboundDto : RefDataBaseDto
    {
        [Required]
        public int AnnualStatisticGroupId { get; set; }
        
    }
}
