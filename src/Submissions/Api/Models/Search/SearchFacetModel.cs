using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Biobanks.Submissions.Api.Models.Search;

public class SearchFacetModel
{
    public string GroupName { get; set; }
    public bool GroupCollapsedByDefault { get; set; }

    public string GroupNameWithoutSpaces => Regex.Replace(GroupName, @"\s+", "");

    public string Name { get; set; }
    public string IndexedName { get; set; }
    public IEnumerable<SearchFacetValueViewModel> FacetValues { get; set; }
}

public class SearchFacetValueViewModel
{
    public string FacetValue { get; set; }
    public string FacetId { get; set; }
    public double? Count { get; set; }
}
