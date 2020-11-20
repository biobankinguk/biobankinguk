using System.Linq;
using AutoMapper;
using Directory.Search.Dto.Facets;
using Directory.Search.Dto.Results;
using Biobanks.Web.Models.Search;
using Castle.Core.Internal;

namespace Biobanks.Web.AutoMapper
{
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
}
