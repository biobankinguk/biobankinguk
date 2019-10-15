namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Base class holding common properties for Reference Data entities.
    /// </summary>
    public abstract class BaseReferenceDatum
    {
        public BaseReferenceDatum(string value)
        {
            Value = value;
        }

        public int Id { get; set; }

        public string Value { get; set; }
    }

    /// <summary>
    /// Base class for Reference Data which has a sort order.
    /// </summary>
    public abstract class SortedBaseReferenceDatum : BaseReferenceDatum
    {
        public SortedBaseReferenceDatum(string value) : base(value)
        {
        }

        public int SortOrder { get; set; }
    }
}
