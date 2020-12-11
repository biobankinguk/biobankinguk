using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class Sex
    {
        public int SexId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }
    }
}
