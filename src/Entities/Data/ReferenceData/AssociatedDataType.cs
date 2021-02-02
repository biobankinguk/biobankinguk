using System.ComponentModel.DataAnnotations;

namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataType
    {
        public int Id { get; set; }

        [Required]
        public string Value { get; set; }

        public string Message { get; set; }
        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

    }
}
