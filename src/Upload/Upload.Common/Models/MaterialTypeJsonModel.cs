using System.Collections.Generic;

namespace Upload.Common.Models
{
    public class MaterialTypeJsonModel
    {
        public string Value { get; set; } = string.Empty;
        public List<string> MaterialTypeGroupNames { get; set; } = new List<string>();
    }
}
