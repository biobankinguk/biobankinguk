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
        public string RouteSlug { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsEnabled { get; set; }

    }
}
