using System.Collections.Generic;
using Biobanks.Data.Entities.ReferenceData;

namespace Biobanks.Data.Entities.Shared.ReferenceData
{
    /// <summary>
    /// This is an internal entity for artificially grouping material types.
    /// It is used for conditional validation against certain material types.
    /// It can be used for any future purposes.
    /// </summary>
    public class MaterialTypeGroup : BaseReferenceData
    {
        /// <summary>
        /// Join entities for MaterialTypes in the group
        /// </summary>
        public ICollection<MaterialType> MaterialTypes { get; set; }
    }
}
