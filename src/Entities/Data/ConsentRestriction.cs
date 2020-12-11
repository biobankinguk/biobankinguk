using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Entities.Data
{
    public class ConsentRestriction
    {
        public int ConsentRestrictionId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }
    }
}
