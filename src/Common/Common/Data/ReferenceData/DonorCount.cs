namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference data. Sets of donor count ranges for a given Sample Set.
    /// </summary>
    public class DonorCount : SortedBaseReferenceDatum
    {
        public int LowerBound { get; set; }
        public int UpperBound { get; set; }
    }
}
