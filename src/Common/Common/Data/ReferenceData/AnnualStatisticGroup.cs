using System.Collections.Generic;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Ref Data. A group which contains a collection of 
    /// </summary>
    public class AnnualStatisticGroup : SortedBaseReferenceDatum
    {
        public AnnualStatisticGroup(string value) : base(value)
        {
        }

        public virtual ICollection<AnnualStatistic> AnnualStatistics { get; set; } = null!;
    }
}
