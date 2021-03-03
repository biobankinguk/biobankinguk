using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Web.Models.ADAC
{
    public class CollectionPointsModel
    {
        public ICollection<CollectionPointModel> CollectionPoints { get; set; }
    }

    public class CollectionPointModel
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public int SortOrder { get; set; }

        public int SampleSetsCount { get; set; }
    }
}