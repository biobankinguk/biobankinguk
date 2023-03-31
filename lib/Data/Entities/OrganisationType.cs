using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities
{
    public class OrganisationType
    {
        public int OrganisationTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

    }
}
