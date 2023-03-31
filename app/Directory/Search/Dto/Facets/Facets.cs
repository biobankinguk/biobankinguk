using System.Collections.Generic;
using System.Linq;

namespace Biobanks.Directory.Search.Dto.Facets
{
    public static class Facets
    {
        private static readonly List<FacetDetailSummary> FacetDetails = new List<FacetDetailSummary>
        {
            new FacetDetailSummary
            {
                Label = "Networks",
                Name = "networks",
                Slug = "nets"
            },
            new FacetDetailSummary
            {
                Label = "Associated Data",
                Name = "associatedData",
                Slug = "assd"
            },
            new FacetDetailSummary
            {
                Label = "Collection Status",
                Name = "collectionStatus",
                Slug = "cols"
            },
            new FacetDetailSummary
            {
                Label = "Consent Restriction",
                Name = "consentRestriction",
                Slug = "cons"
            },
            new FacetDetailSummary
            {
                Label = "Age Range",
                Name = "ageRange",
                Slug = "ager"
            },
            new FacetDetailSummary
            {
                Label = "Donor Count",
                Name = "donorCount",
                Slug = "donc"
            },
            new FacetDetailSummary
            {
                Label = "Sex",
                Name = "sex",
                Slug = "sex"
            },
            new FacetDetailSummary
            {
                Label = "Material Type",
                Name = "materialType",
                Slug = "matt"
            },
            new FacetDetailSummary
            {
                Label = "Extraction Procedure",
                Name = "extractionProcedure",
                Slug = "extprod"
            },
            new FacetDetailSummary
            {
                Label = "Storage Temperature",
                Name = "storageTemperature",
                Slug = "stmp"
            },
            new FacetDetailSummary
            {
                Label = "Macroscopic Assessment",
                Name = "macroscopicAssessment",
                Slug = "macr"
            },
            new FacetDetailSummary
            {
                Label = "Protocols",
                Name = "protocols",
                Slug = "prot"
            },
            new FacetDetailSummary
            {
                Label = "Annual Donor Expectation",
                Name = "annualDonorExpectation",
                Slug = "ande"
            },
             new FacetDetailSummary
            {
                Label = "Country",
                Name = "country",
                Slug = "ctry"
            },
             new FacetDetailSummary
            {
                Label = "County",
                Name = "county",
                Slug = "cty"
            }
        };

        public static string GetFacetLabel(string facetName) => FacetDetails.First(x => x.Name == facetName)?.Label;

        public static string GetFacetSlug(string facetName) => FacetDetails.First(x => x.Name == facetName)?.Slug;

        public static string GetFacetName(string facetSlug) => FacetDetails.First(x => x.Slug == facetSlug)?.Name;
    }
}
