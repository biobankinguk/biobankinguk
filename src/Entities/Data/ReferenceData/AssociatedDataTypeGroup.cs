using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Entities.Data
{
    public class AssociatedDataTypeGroup
    {
        public int AssociatedDataTypeGroupId { get; set; }
        [Required]
        public string Description { get; set; }

        public virtual ICollection<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
