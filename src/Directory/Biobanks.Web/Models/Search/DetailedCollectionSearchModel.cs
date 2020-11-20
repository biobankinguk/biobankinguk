using System.Collections.Generic;

namespace Biobanks.Web.Models.Search
{
    public class DetailedCollectionSearchModel
    {
        public string Diagnosis { get; set; }
        public string SelectedFacets { get; set; }

        public int BiobankId { get; set; }
        public string BiobankExternalId { get; set; }
        public string BiobankName { get; set; }

        public string LogoName { get; set; }

        public IList<DetailedCollectionSearchCollectionModel> Collections { get; set; }
    }
}