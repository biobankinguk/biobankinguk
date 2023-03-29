using System.Collections.Generic;
using System.Linq;
using Biobanks.Directory.Search.Constants;
using Biobanks.Directory.Search.Dto.Documents;
using Nest;

namespace Biobanks.Directory.Search.Dto.Facets
{
    public static class FacetList
    {
        private static readonly List<FacetGroup> FacetGroups = new List<FacetGroup>
        {
            new FacetGroup
            {
                Id = FacetGroupId.SampleDetails,
                Name = "Sample Details",
                CollapsedByDefault = false,
                SortOrder = 1
            },
            new FacetGroup
            {
                Id = FacetGroupId.Data,
                Name = "Data",
                CollapsedByDefault = false,
                SortOrder = 2
            },
            new FacetGroup
            {
                Id = FacetGroupId.Donors,
                Name = "Donors",
                CollapsedByDefault = false,
                SortOrder = 3
            },
            new FacetGroup
            {
                Id = FacetGroupId.BiobankServices,
                Name = "Biobank Services",
                CollapsedByDefault = false,
                SortOrder = 4
            },
            new FacetGroup
            {
                Id = FacetGroupId.Networks,
                Name = "Networks",
                CollapsedByDefault = true,
                SortOrder = 5
            },
            new FacetGroup
            {
                Id = FacetGroupId.CollectionDetails,
                Name = "Collection Details",
                CollapsedByDefault = false,
                SortOrder = 6
            },
            //new FacetGroup
            //{
            //    Id = FacetGroupId.Governance,
            //    Name = "Governance",
            //    CollapsedByDefault = true,
            //    SortOrder = 7
            //},
            new FacetGroup
            {
                Id = FacetGroupId.AccessConditions,
                Name = "Access Conditions",
                CollapsedByDefault = false,
                SortOrder = 8
            },
            new FacetGroup
            {
                Id = FacetGroupId.Location,
                Name = "Location",
                CollapsedByDefault = false,
                SortOrder = 9
            }
        };

        private static readonly List<FacetDetail> FacetDetails = new List<FacetDetail>
        {
            new FacetDetail(FacetGroupId.Networks)
            {
                Label = "Networks",
                Name = "networks",
                Slug = "nets",
                NestedAggregation = true,
                NestedAggregationPath = "networks",
                NestedAggregationFieldName = "name",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection,
                    SearchType.Capability
                },
                SortOrderWithinGroup = 1
            },
            new FacetDetail(FacetGroupId.AccessConditions)
            {
                Label = "Access Condition",
                Name = "accessCondition",
                Slug = "acc",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 3,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.Data)
            {
                Label = "Associated Data",
                Name = "associatedData",
                Slug = "assd",
                NestedAggregation = true,
                NestedAggregationPath = "associatedData",
                NestedAggregationFieldName = "text",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection,
                    SearchType.Capability
                },
                SortOrderWithinGroup = 1
            },
            new FacetDetail(FacetGroupId.Data)
            {
                Label = "Timeframe",
                Name = "timeframe",
                Slug = "asdt",
                NestedAggregation = true,
                NestedAggregationPath = "associatedData",
                NestedAggregationFieldName = "timeframe",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection,
                    SearchType.Capability
                },
                SortOrderWithinGroup = 2,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.BiobankServices)
            {
                Label = "Biobank Services",
                Name = "biobankServices",
                Slug = "bis",
                NestedAggregation = true,
                NestedAggregationPath = "biobankServices",
                NestedAggregationFieldName = "name",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 1
            },
            new FacetDetail(FacetGroupId.AccessConditions)
            {
                Label = "Collection Status",
                Name = "collectionStatus",
                Slug = "cols",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 2
            },
            new FacetDetail(FacetGroupId.AccessConditions)
            {
                Label = "Consent Restriction",
                Name = "consentRestrictions",
                Slug = "cons",
                NestedAggregation = true,
                NestedAggregationPath = "consentRestrictions",
                NestedAggregationFieldName = "description",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 1
            },
            new FacetDetail(FacetGroupId.CollectionDetails)
            {
                Label = "Collection Type",
                Name = "collectionType",
                Slug = "clt",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 2
            },
            new FacetDetail(FacetGroupId.Donors)
            {
                Label = "Age Range",
                Name = "ageRange",
                Slug = "ager",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 3,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.Donors)
            {
                Label = "Donor Count",
                Name = "donorCount",
                Slug = "donc",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 1,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.Donors)
            {
                Label = "Sex",
                Name = "sex",
                Slug = "sex",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 2,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.SampleDetails)
            {
                Label = "Material Type",
                Name = "materialType",
                Slug = "matt",
                NestedAggregation = true,
                NestedAggregationPath = "materialPreservationDetails",
                NestedAggregationFieldName = "materialType",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 1
            },
            new FacetDetail(FacetGroupId.SampleDetails)
            {
                Label = "Storage Temperature",
                Name = "storageTemperature",
                Slug = "stmp",
                NestedAggregation = true,
                NestedAggregationPath = "materialPreservationDetails",
                NestedAggregationFieldName = "storageTemperature",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 3,
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.SampleDetails)
            {
                Label = "Macroscopic Assessment",
                Name = "macroscopicAssessment",
                Slug = "macr",
                NestedAggregation = true,
                NestedAggregationPath = "materialPreservationDetails",
                NestedAggregationFieldName = "macroscopicAssessment",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 2
            },
            new FacetDetail(FacetGroupId.SampleDetails)
            {
                Label = "Preservation Type",
                Name = "preservationType",
                Slug = "prsv",
                NestedAggregation = true,
                NestedAggregationPath = "materialPreservationDetails",
                NestedAggregationFieldName = "preservationType",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 4
            },
            new FacetDetail(FacetGroupId.SampleDetails)
            {
                Label = "Extraction Procedure",
                Name = "extractionProcedure",
                Slug = "extprod",
                NestedAggregation = true,
                NestedAggregationPath = "materialPreservationDetails",
                NestedAggregationFieldName = "extractionProcedure",
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                },
                SortOrderWithinGroup = 5
            },
            new FacetDetail
            {
                Label = "Protocols",
                Name = "protocols",
                Slug = "prot",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Capability
                }
            },
            new FacetDetail
            {
                Label = "Annual Donor Expectation",
                Name = "annualDonorExpectation",
                Slug = "ande",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Capability
                },
                HasMetadata = true
            },
            new FacetDetail(FacetGroupId.Location)
            {
                Label = "Country",
                Name = "country",
                Slug = "ctry",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                }
            },
            new FacetDetail(FacetGroupId.Location)
            {
                Label = "County",
                Name = "county",
                Slug = "cty",
                NestedAggregation = false,
                SearchTypes = new List<SearchType>
                {
                    SearchType.Collection
                }
            }
        };

        public static FacetGroup GetFacetGroup(FacetGroupId facetGroupId)
            => FacetGroups.FirstOrDefault(x => x.Id == facetGroupId);

        public static IEnumerable<FacetDetail> GetFacetDetails() => FacetDetails.ToList();

        public static FacetDetail GetFacetDetail(string facetName) => FacetDetails.FirstOrDefault(x => x.Name == facetName);

        public static IEnumerable<FacetDetail> GetFacetDetails(SearchType searchType)
            => FacetDetails.Where(x => x.SearchTypes.Contains(searchType)).ToList();

        public static string GetFacetLabel(string facetName)
            => FacetDetails.FirstOrDefault(x => x.Name == facetName)?.Label;

        public static string GetFacetSlug(string facetName)
            => FacetDetails.FirstOrDefault(x => x.Name == facetName)?.Slug;

        public static string GetFacetName(string facetSlug)
            => FacetDetails.FirstOrDefault(x => x.Slug == facetSlug)?.Name;

        public static SearchType GetSearchType<T>(ISearchResponse<T> searchResponse) where T : class
            => typeof(SearchResponse<CollectionDocument>) == searchResponse.GetType()
                ? SearchType.Collection
                : SearchType.Capability;

        public static SearchType GetSearchType<T>(AggregationContainerDescriptor<T> aggregationDescriptor) where T : class
            => typeof(AggregationContainerDescriptor<CollectionDocument>) == aggregationDescriptor.GetType()
                ? SearchType.Collection
                : SearchType.Capability;
    }
}
