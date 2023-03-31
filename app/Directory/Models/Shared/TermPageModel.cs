using System.Collections.Generic;
using Biobanks.Directory.Models.Search;

namespace Biobanks.Directory.Models.Shared;

public class TermPageModel
{
    public IEnumerable<ReadOntologyTermModel> OntologyTermsModel { get; set; }
    public TermpageContentModel TermpageContentModel { get; set; }
}
