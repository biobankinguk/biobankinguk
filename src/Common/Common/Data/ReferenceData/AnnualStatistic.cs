namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference Data. Annual Statistics for a given organisation
    /// </summary>
    public class AnnualStatistic : BaseReferenceDatum
    {
        public virtual AnnualStatisticGroup annualStatisticGroup { get; set; }
    }
}
