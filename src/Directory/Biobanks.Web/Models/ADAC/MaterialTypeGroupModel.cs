using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class MaterialTypeGroupModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int MaterialDetailCount { get; set; }

        public IEnumerable<string> MaterialTypes { get; set; }
    }
}