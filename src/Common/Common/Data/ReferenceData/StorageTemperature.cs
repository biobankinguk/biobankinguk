namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference Data. Temperature ranges for a given MaterialDetail.
    /// </summary>
    public class StorageTemperature : SortedBaseReferenceDatum
    {
        public string? Minimum { get; set; }
        public string? Maximum { get; set; }
    }
}
