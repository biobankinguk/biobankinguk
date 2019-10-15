namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Navigation entity. Links MaterialType and MaterialTypeGroup together to allow for a many to many relationship.
    /// </summary>
    public class MaterialTypeGroupMaterialType
    {
        public int MaterialTypeId { get; set; }
        public virtual MaterialType MaterialType { get; set; } = null!;
        public int MaterialTypeGroupId { get; set; }
        public virtual MaterialTypeGroup MaterialTypeGroup { get; set; } = null!;
    }
}
