using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace Directory.Entity.Data
{
    public class AssociatedDataTypeGroup
    {
        public int AssociatedDataTypeGroupId { get; set; }
        [Required]
        public string Description { get; set; }

        public virtual ICollection<AssociatedDataType> AssociatedDataTypes { get; set; }
    }
}
