using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class CollectionType
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public int SortOrder { get; set; }
        public virtual ICollection<Organisation> Organisations { get; set; }
    }
}
