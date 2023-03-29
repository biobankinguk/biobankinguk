using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Biobanks.Entities.Data.ReferenceData;
using System;

namespace Biobanks.Entities.Data
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
