namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Navigation entity. Links MaterialType and MaterialTypeGroup together to allow for a many to many relationship.
    /// </summary>
    public class MaterialTypeGroupMaterialType
    {
        public int MaterialTypeId { get; set; }
        public MaterialType MaterialType { get; set; }
        public int MaterialTypeGroupId { get; set; }
        public MaterialTypeGroup MaterialTypeGroup { get; set; }
    }
}
