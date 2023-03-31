using System;
using System.ComponentModel.DataAnnotations;

namespace Biobanks.Data.Entities
{
    public class ContentPage
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(128)]
        public string RouteSlug { get; set; }
        [MaxLength(250)]
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsEnabled { get; set; }

    }
}
