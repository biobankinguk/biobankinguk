using System.Collections.Generic;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Ref Data. Category in which a Material Type can belong.
    /// </summary>
    public class MaterialTypeGroup : BaseReferenceDatum
    {

        public virtual ICollection<MaterialTypeGroupMaterialType> MaterialTypeGroupMaterialTypes { get; set; } = null!;
    }
}
