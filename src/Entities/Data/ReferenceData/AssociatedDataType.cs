namespace Biobanks.Entities.Data.ReferenceData
{
    public class AssociatedDataType : ReferenceDataBase
    {
        public string Message { get; set; }

        public int AssociatedDataTypeGroupId { get; set; }

        public virtual AssociatedDataTypeGroup AssociatedDataTypeGroup { get; set; }

    }
}
