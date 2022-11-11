using System.Collections.Generic;
using Biobanks.Submissions.Api.Models.Search;

namespace Biobanks.Submissions.Api.Models.Shared;

public class TermPageModel
{
    public IEnumerable<ReadOntologyTermModel> OntologyTermsModel { get; set; }
    public TermpageContentModel TermpageContentModel { get; set; }
}
