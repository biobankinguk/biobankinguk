using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Submissions.Api.Models.Shared
{
    public class MaterialTypeModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public IEnumerable<string> MaterialTypeGroups { get; set; }
    }
}

