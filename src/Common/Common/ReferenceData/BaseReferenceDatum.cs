namespace Common.ReferenceData
{
    /// <summary>
    /// Base class holding common properties for Reference Data entities.
    /// </summary>
    public class BaseReferenceDatum
    {
        public int Id { get; set; }

        public string Value { get; set; }
    }

    /// <summary>
    /// Base class for Reference Data which has a sort order.
    /// </summary>
    public class SortedBaseReferenceDatum : BaseReferenceDatum
    {
        public int SortOrder { get; set; }
    }
}
