using System;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO
{
    public class AnnualStatisticDto : RefDataBaseDto
    {
        [Required]
        public int AnnualStatisticGroupId { get; set; }
        //Name of the group to which this AnnualStatistic belongs
        public string Group { get; set; } = string.Empty;
    }
}
