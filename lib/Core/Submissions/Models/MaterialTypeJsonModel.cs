using System.Collections.Generic;

namespace Core.Submissions.Models
{
    public class MaterialTypeJsonModel
    {
        public string Value { get; set; }
        public ICollection<string> MaterialTypeGroupNames { get; set; }
    }
}
