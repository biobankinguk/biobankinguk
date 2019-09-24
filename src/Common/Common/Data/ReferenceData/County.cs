namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference data. Geographic region within a country.
    /// </summary>
    public class County : BaseReferenceDatum
    {
        public virtual Country Country { get; set; }
    }
}
