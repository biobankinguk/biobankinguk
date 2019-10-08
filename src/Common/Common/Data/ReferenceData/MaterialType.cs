using System.Collections.Generic;

namespace Common.Data.ReferenceData
{
    /// <summary>
    /// Reference data. The type of material for a given MaterialDetail.
    /// </summary>
    public class MaterialType : SortedBaseReferenceDatum
    {
        public virtual ICollection<MaterialTypeGroup> MaterialTypeGroups { get; set; }
    }
}
