using System.Linq;
using AutoMapper;
using Biobanks.Search.Dto.Facets;
using Biobanks.Search.Dto.Results;
using Biobanks.Submissions.Api.Extensions;
using Biobanks.Submissions.Api.Models.Search;

namespace Biobanks.Submissions.Api.MappingProfiles;

public class BaseSearchProfile : Profile
{
  public BaseSearchProfile()
  {
    CreateMap<Result, BaseSearchModel>().ConvertUsing<BaseSearchModelTypeConverter>();
  }
}

public class BaseSearchModelTypeConverter : ITypeConverter<Result, BaseSearchModel>
{

  public BaseSearchModel Convert(Result source, BaseSearchModel destination, ResolutionContext context)
  {

    destination.Biobanks = source.Biobanks.Select(x => new BiobankSearchSummaryModel
    {
      ExternalId = x.ExternalId,
      Name = x.Name,
      CollectionCount = x.CollectionCount,
      SampleSetSummaries = x.SampleSetSummaries
    });

    destination.Facets = source.Facets
      .Where(f => f.Values.Any())
      .OrderBy(x => x.GroupOrder)
      .ThenBy(x => x.FacetOrder)
      .Select(x =>
      {
        var searchFacetModel = new SearchFacetModel
        {
          GroupName = x.GroupName,
          GroupCollapsedByDefault = x.GroupCollapsedByDefault,
          Name = FacetList.GetFacetLabel(x.Name),
          IndexedName = x.Name,
          FacetValues = x.Values
            .OrderBy(o => int.Parse(o.SortOrder.IsNullOrEmpty() ? "0" : o.SortOrder))
            .ThenBy(o => o.Name)
            .Select(y => new SearchFacetValueViewModel
            {
              FacetValue = y.Name,
              FacetId = BuildFacetId(x.Name, y.Name),
              Count = y.Value
            })
        };

        return searchFacetModel;
      });

    return destination;
  }

  private static string BuildFacetId(string indexedName, string facetValue)
  {
    return string.Concat(FacetList.GetFacetSlug(indexedName), "_", facetValue);
  }
  
}
