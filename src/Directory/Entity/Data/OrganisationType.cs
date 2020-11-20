using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class OrganisationType
    {
        public int OrganisationTypeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

    }
}
