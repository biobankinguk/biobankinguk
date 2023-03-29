using System.Collections.Generic;
using Biobanks.Directory.Models.Shared;
using Biobanks.Directory.Search.Constants;

namespace Biobanks.Directory.Models.Search;

public class NoResultsModel
{
    public SearchDocumentType SearchType { get; set; }
        
    public string OntologyTerm { get; set; }

    public ICollection<OntologyTermModel> Suggestions { get; set; }
}
