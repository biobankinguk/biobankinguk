using System.Collections.Generic;

namespace Biobanks.Directory.Models.Profile
{
    public class BiobankPublicationsModel
    {
        public string ExternalId { get; set; }
        public bool ExcludePublications { get; set; }

        public ICollection<BiobankPublicationModel> Publications { get; set; }
    }

    public class BiobankPublicationModel
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public int? Year { get; set; }
        public string Journal { get; set; }
        public string DOI { get; set; }
    }
}
