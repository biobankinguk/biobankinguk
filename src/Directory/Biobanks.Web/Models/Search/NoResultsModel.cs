using System.Collections.Generic;
using Biobanks.Search.Constants;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Search
{
    public class NoResultsModel
    {
        public SearchDocumentType SearchType { get; set; }
        
        public string OntologyTerm { get; set; }

        public ICollection<OntologyTermModel> Suggestions { get; set; }
    }
}
