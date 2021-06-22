using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Biobanks.Search.Legacy;
using Biobanks.Search.Dto.Facets;
using Biobanks.Search.Constants;
using Biobanks.Services.Contracts;
using Biobanks.Web.Models.Search;
using Biobanks.Web.Models.Shared;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Biobanks.Web.Extensions;
using Biobanks.Web.Results;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class SearchController : Controller
    {
        private readonly ISearchProvider _searchProvider;
        private readonly IMapper _mapper;
        private readonly IBiobankReadService _biobankReadService;

        public SearchController(
            ISearchProvider searchProvider,
            IMapper mapper,
            IBiobankReadService biobankReadService)
        {
            _searchProvider = searchProvider;
            _mapper = mapper;
            _biobankReadService = biobankReadService;
        }

        [HttpGet]
        public async Task<ViewResult> Collections(string ontologyTerm, string selectedFacets)
        {
            // Check If Valid and Visible Term
            if (!string.IsNullOrWhiteSpace(ontologyTerm))
            {
                var term = await _biobankReadService.GetOntologyTermByDescription(ontologyTerm);

                if (term is null)
                {
                    return await NoResults(new NoResultsModel
                    {
                        OntologyTerm = ontologyTerm,
                        SearchType = SearchDocumentType.Collection
                    });
                }
            }

            // Build the base model.
            var model = new BaseSearchModel
            {
                OntologyTerm = ontologyTerm,
                // Extract the search facets.
                SelectedFacets = ExtractSearchFacets(selectedFacets)
            };

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchByOntologyTerm(
                ontologyTerm,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    OntologyTerm = model.OntologyTerm,
                    SearchType = SearchDocumentType.Collection
                });

            model.Countries = (await _biobankReadService.ListCountriesAsync())
                .ToDictionary(
                    x => x.Value,
                    x => x.Counties.Select(y => y.Value).ToList()
                );
            return View(model);
        }

        private async Task<ViewResult> NoResults(NoResultsModel model)
        {
            model.Suggestions = await GetOntologyTermSearchResultsAsync(model.SearchType, model.OntologyTerm?.ToLower());

            //BIO-455 special case for cancer (will override this with a genericised approach in BIO-447
            if (string.Equals(model.OntologyTerm, "cancer", StringComparison.CurrentCultureIgnoreCase))
            {
                //get suggestions for the relevant correct searches
                var malignant = await GetOntologyTermSearchResultsAsync(model.SearchType, "malignant");
                var neoplasm = await GetOntologyTermSearchResultsAsync(model.SearchType, "neoplasm");

                //munge them into a distinct list
                var results = new List<OntologyTermModel>();
                results.AddRange(malignant);
                results.AddRange(neoplasm);
                model.Suggestions = results
                    .DistinctBy(x => x.Description)
                    .OrderBy(x => x.Description)
                    .ToList();
            }

            return View("NoResults", model);
        }

        [HttpGet]
        public async Task<ViewResult> CollectionsDetail(string biobankExternalId, string ontologyTerm, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchByOntologyTermAndBiobank(biobankExternalId, ontologyTerm, searchFacets);

            var model = _mapper.Map<DetailedCollectionSearchModel>(searchResults);

            model.OntologyTerm = ontologyTerm;
            model.SelectedFacets = selectedFacets;

            // Get the biobank logo name from the database.
            model.LogoName = (await _biobankReadService.GetBiobankByExternalIdAsync(biobankExternalId)).Logo;

            //Get Collection Descriptions in bulk
            var descriptions =
                await _biobankReadService.GetDescriptionsByCollectionIds(
                    model.Collections.Select(x => x.CollectionId));

            foreach (var collection in model.Collections)
            {
                collection.Description = descriptions[collection.CollectionId];
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> Capabilities(string ontologyTerm, string selectedFacets)
        {
            // Check If Valid and Visible Term
            if (!string.IsNullOrEmpty(ontologyTerm))
            {
                var term = await _biobankReadService.GetOntologyTermByDescription(ontologyTerm);

                if (term is null)
                {
                    return await NoResults(new NoResultsModel
                    {
                        OntologyTerm = ontologyTerm,
                        SearchType = SearchDocumentType.Collection
                    });
                }
            }

            // Build the base model.
            var model = new BaseSearchModel
            {
                OntologyTerm = ontologyTerm
            };

            // Extract the search facets.
            model.SelectedFacets = ExtractSearchFacets(selectedFacets);

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchByOntologyTerm(
                ontologyTerm,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    OntologyTerm = model.OntologyTerm,
                    SearchType = SearchDocumentType.Collection
                });

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> CapabilitiesDetail(string biobankExternalId, string ontologyTerm, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchByOntologyTermAndBiobank(biobankExternalId, ontologyTerm, searchFacets);

            var model = _mapper.Map<DetailedCapabilitySearchModel>(searchResults);

            model.OntologyTerm = ontologyTerm;
            model.SelectedFacets = selectedFacets;

            // Get the biobank logo name from the database.
            model.LogoName = (await _biobankReadService.GetBiobankByExternalIdAsync(biobankExternalId)).Logo;

            return View(model);
        }


        #region Diagnosis Type Ahead
        [AllowAnonymous]
        public async Task<JsonpResult> ListOntologyTerms(string wildcard, string callback)
        {
            var ontologyTermModels = await GetOntologyTermsAsync(wildcard);

            return this.Jsonp(ontologyTermModels, callback, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonpResult> SearchOntologyTerms(string searchDocumentType, string wildcard, string callback)
        {
            SearchDocumentType type;

            switch (searchDocumentType)
            {
                case "Collections":
                    type = SearchDocumentType.Collection;
                    break;
                case "Capabilities":
                    type = SearchDocumentType.Capability;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var ontologyTermModel = await GetOntologyTermSearchResultsAsync(type, wildcard.ToLower());

            return this.Jsonp(ontologyTermModel, callback, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<OntologyTermModel>> GetOntologyTermSearchResultsAsync(SearchDocumentType type, string wildcard)
        {
            var searchOntologyTerms = _searchProvider.ListOntologyTerms(type, wildcard);
            var directoryOntologyTerms = await _biobankReadService.ListOntologyTermsAsync();

            // Join Ontology Terms In Search and Directory Based On Ontology Term Value
            var model = directoryOntologyTerms.Join(searchOntologyTerms, 
                outer => outer.Value.ToLower(), 
                inner => inner.OntologyTerm, 
                (directoryTerm, searchTerm) =>
                {
                    var nonMatchingTerms = directoryTerm.OtherTerms?
                        .Split(',')
                        .Select(m => m.Trim())
                        .Where(m => !searchTerm.MatchingOtherTerms.Contains(m))
                        .ToList();

                    return new OntologyTermModel
                    {
                        OntologyTermId = directoryTerm.Id,
                        Description = directoryTerm.Value,
                        OtherTerms = directoryTerm.OtherTerms ?? "",
                        DisplayOnDirectory = directoryTerm.DisplayOnDirectory,
                        MatchingOtherTerms = searchTerm.MatchingOtherTerms,
                        NonMatchingOtherTerms = nonMatchingTerms ?? new List<string>()
                    };
                });

            return model.ToList();
        }

        private async Task<List<OntologyTermModel>> GetOntologyTermsAsync(string wildcard)
        {
            var ontologyTerms = await _biobankReadService.ListOntologyTermsAsync(wildcard);

            var model = ontologyTerms.Select(x =>
               new OntologyTermModel
               {
                   OntologyTermId = x.Id,
                   Description = x.Value,
                   OtherTerms = x.OtherTerms,
                   DisplayOnDirectory = x.DisplayOnDirectory
               }
            )
            .ToList();

            return model;
        }
        #endregion

        #region Utility functions related directly to search
        private static IList<string> ExtractSearchFacets(string selectedFacets)
        {
            if (string.IsNullOrEmpty(selectedFacets))
                return new List<string>();

            return (List<string>)JsonConvert.DeserializeObject(selectedFacets, typeof(List<string>));
        }

        private static IEnumerable<SelectedFacet> BuildSearchFacets(IEnumerable<string> facetIds)
        {
            return facetIds.Select(x => new SelectedFacet
            {
                Name = FacetList.GetFacetName(GetFacetSlug(x)),
                Value = GetFacetValue(x)
            }).Where(x => !(x.Name is null));
        }

        private static string GetFacetValue(string facetId)
        {
            return facetId.Substring(facetId.IndexOf('_') + 1);
        }

        private static string GetFacetSlug(string facetId)
        {
            return facetId.SubstringUpToFirst('_');
        }
        #endregion

        //this allows us to specify Collections / Capabilities with one box, using radios
        public ActionResult Unified(string ontologyTerm, string searchRadio)
        {
            switch (searchRadio)
            {
                case "Collections":
                    return RedirectToAction("Collections", new {ontologyTerm = ontologyTerm});
                case "Capabilities":
                    return RedirectToAction("Capabilities", new {ontologyTerm = ontologyTerm});
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    }
}
