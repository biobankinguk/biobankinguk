namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference data. Geographic region within a country.
    /// </summary>
    public class County : BaseReferenceDatum
    {
        public County(string value) : base(value)
        {
        }

        public virtual Country Country { get; set; } = null!;
    }
}
