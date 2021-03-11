using System.Collections.Generic;

namespace Biobanks.Common.Models
{
    public class MaterialTypeJsonModel
    {
        public string Value { get; set; }
        public ICollection<string> MaterialTypeGroupNames { get; set; }
    }
}
