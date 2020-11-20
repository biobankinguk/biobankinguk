using System.Collections.Generic;

namespace Biobanks.Web.Models.Biobank
{
    public class BiobankPublicationsModel
    {
        public ICollection<BiobankPublicationModel> BiobankPublicationModels { get; set; }
    }

    public class BiobankPublicationModel
    {
        public string PublicationId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public int Year { get; set; }
        public string Journal { get; set; }
        public string DOI { get; set; }
        public bool? Approved { get; set; }
    }
}