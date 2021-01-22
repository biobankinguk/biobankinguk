using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataTypeGroup
    {
        public int AssociatedDataTypeGroupId { get; set; }
        [Required]
        public string Description { get; set; }

        public virtual ICollection<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
