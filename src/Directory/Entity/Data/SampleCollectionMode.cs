using System.ComponentModel.DataAnnotations;

namespace Directory.Entity.Data
{
    public class SampleCollectionMode
    {
        public int SampleCollectionModeId { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

    }
}
