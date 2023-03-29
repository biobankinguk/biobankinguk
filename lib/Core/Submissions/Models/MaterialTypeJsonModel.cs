using System.Collections.Generic;

namespace Biobanks.Submissions.Models
{
    public class MaterialTypeJsonModel
    {
        public string Value { get; set; }
        public ICollection<string> MaterialTypeGroupNames { get; set; }
    }
}
