using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataType
    {
        public int AssociatedDataTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public string Message { get; set; }
        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

    }
}
