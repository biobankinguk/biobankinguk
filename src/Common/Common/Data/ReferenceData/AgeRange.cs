using System.ComponentModel.DataAnnotations;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference Data. Age range with a maximum and minmum value for a given sample.
    /// </summary>
    public class AgeRange : SortedBaseReferenceDatum
    {
        /// <summary>
        /// Lowest month value for a given range. Cannot be less than -9 months.
        /// </summary>
        [Range(-9, int.MaxValue)]
        public int? MinMonth { get; set; }
        /// <summary>
        /// Highest month value for a given range. No upper limit.
        /// </summary>
        public int? MaxMonth { get; set; }
    }
}
