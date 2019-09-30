namespace Common.DTO
{
    /// <summary>
    /// Base DTO for ref data, for use when handling ref data when no ID value is applicable.
    /// </summary>
    public class RefDataBaseDto
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// Base DTO which adds sort order when its applicable.
    /// </summary>
    public class SortedRefDataBaseDto : RefDataBaseDto
    {
        public int SortOrder { get; set; }
    }
}
