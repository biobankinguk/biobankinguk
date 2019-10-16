namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Base class holding common properties for Reference Data entities.
    /// </summary>
    public abstract class BaseReferenceDatum
    {
        public int Id { get; set; }

        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Base class for Reference Data which has a sort order.
    /// </summary>
    public abstract class SortedBaseReferenceDatum : BaseReferenceDatum
    {
        public int SortOrder { get; set; }
    }
}
