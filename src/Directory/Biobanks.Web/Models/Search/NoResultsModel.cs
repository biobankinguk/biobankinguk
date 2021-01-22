using System.Collections.Generic;
using Biobanks.Search.Constants;
using Biobanks.Web.Models.Shared;

namespace Biobanks.Web.Models.Search
{
    public class NoResultsModel
    {
        public SearchDocumentType SearchType { get; set; }
        
        public string SnomedTerm { get; set; }

        public ICollection<SnomedTermModel> Suggestions { get; set; }
    }
}
