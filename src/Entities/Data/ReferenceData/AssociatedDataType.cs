namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataType : BaseReferenceData
    {
        public string Message { get; set; }

        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

    }
}
