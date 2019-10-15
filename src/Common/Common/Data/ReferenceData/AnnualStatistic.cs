namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference Data. Annual Statistics for a given organisation
    /// </summary>
    public class AnnualStatistic : BaseReferenceDatum
    {
        public AnnualStatistic(string value) : base(value)
        {
        }

        public virtual AnnualStatisticGroup AnnualStatisticGroup { get; set; } = null!;
    }
}
