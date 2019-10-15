namespace Common.DTO
{
    /// <summary>
    /// Base DTO for ref data, for use when handling ref data when no ID value is applicable.
    /// </summary>
    public class RefDataBaseDto
    {
        public string Value { get; set; } = null!;
    }

    public class SortedRefDataBaseDto : RefDataBaseDto
    {
        public int SortOrder { get; set; }
    }
}
