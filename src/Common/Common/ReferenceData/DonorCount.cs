namespace Common.ReferenceData
{
    /// <summary>
    /// Reference data. Sets of donor count ranges for a given Sample Set.
    /// </summary>
    public class DonorCount : SortedBaseReferenceDatum
    {
        /// <summary>
        /// Minimum value of the Donor Count range.
        /// </summary>
        public int LowerBound { get; set; }
        
        /// <summary>
        /// Maximum value of the Donor Count range.
        /// </summary>
        public int? UpperBound { get; set; }
    }
}
