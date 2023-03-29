using System.Collections.Generic;
using Biobanks.Search.Constants;
using Biobanks.Submissions.Api.Models.Shared;

namespace Biobanks.Submissions.Api.Models.Search;

public class NoResultsModel
{
    public SearchDocumentType SearchType { get; set; }
        
    public string OntologyTerm { get; set; }

    public ICollection<OntologyTermModel> Suggestions { get; set; }
}
