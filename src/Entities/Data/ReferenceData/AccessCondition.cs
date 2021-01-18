using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class AccessCondition
    {
        public int AccessConditionId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
