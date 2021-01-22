using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Biobanks.Search.Legacy;
using Biobanks.Search.Dto.Facets;
using Biobanks.Search.Constants;
using Directory.Services.Contracts;
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
        public async Task<ViewResult> Collections(string snomedTerm, string selectedFacets)
        {
            // Build the base model.
            var model = new BaseSearchModel
            {
                SnomedTerm = snomedTerm ?? " "  // Null values set to " " wildcard
            };

            // Extract the search facets.
            model.SelectedFacets = ExtractSearchFacets(selectedFacets);

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchBySnomedTerm(
                snomedTerm,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    SnomedTerm = model.SnomedTerm,
                    SearchType = SearchDocumentType.Collection
                });

            return View(model);
        }

        private async Task<ViewResult> NoResults(NoResultsModel model)
        {
            model.Suggestions = await GetSnomedTermSearchResultsAsync(model.SearchType, model.SnomedTerm?.ToLower());

            //BIO-455 special case for cancer (will override this with a genericised approach in BIO-447
            if (model.SnomedTerm.ToLower() == "cancer")
            {
                //get suggestions for the relevant correct searches
                var malignant = await GetSnomedTermSearchResultsAsync(model.SearchType, "malignant");
                var neoplasm = await GetSnomedTermSearchResultsAsync(model.SearchType, "neoplasm");

                //munge them into a distinct list
                var results = new List<SnomedTermModel>();
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
        public async Task<ViewResult> CollectionsDetail(string biobankExternalId, string snomedTerm, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchBySnomedTermAndBiobank(biobankExternalId, snomedTerm, searchFacets);

            var model = _mapper.Map<DetailedCollectionSearchModel>(searchResults);

            model.SnomedTerm = snomedTerm;
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
        public async Task<ViewResult> Capabilities(string snomedTerm, string selectedFacets)
        {
            // Build the base model.
            var model = new BaseSearchModel
            {
                SnomedTerm = snomedTerm ?? " "
            };

            // Extract the search facets.
            model.SelectedFacets = ExtractSearchFacets(selectedFacets);

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchBySnomedTerm(
                snomedTerm,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    SnomedTerm = model.SnomedTerm,
                    SearchType = SearchDocumentType.Collection
                });

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> CapabilitiesDetail(string biobankExternalId, string snomedTerm, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchBySnomedTermAndBiobank(biobankExternalId, snomedTerm, searchFacets);

            var model = _mapper.Map<DetailedCapabilitySearchModel>(searchResults);

            model.Diagnosis = snomedTerm;
            model.SelectedFacets = selectedFacets;

            // Get the biobank logo name from the database.
            model.LogoName = (await _biobankReadService.GetBiobankByExternalIdAsync(biobankExternalId)).Logo;

            return View(model);
        }

        #region Diagnosis Type Ahead
        [AllowAnonymous]
        public async Task<JsonpResult> ListSnomedTerms(string wildcard, string callback)
        {
            var snomedTermModels = await GetSnomedTermsAsync(wildcard);

            return this.Jsonp(snomedTermModels, callback, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonpResult> SearchSnomedTerms(string searchDocumentType, string wildcard, string callback)
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

            var snomedTermsModel = await GetSnomedTermSearchResultsAsync(type, wildcard.ToLower());

            return this.Jsonp(snomedTermsModel, callback, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<SnomedTermModel>> GetSnomedTermSearchResultsAsync(SearchDocumentType type, string wildcard)
        {
            var snomedTerms = await _biobankReadService.ListSearchableSnomedTermsAsync(type, wildcard);

            var model =  snomedTerms.Select(x =>
                new SnomedTermModel
                {
                    SnomedTermId = x.Id,
                    Description = x.Description,
                    OtherTerms = x.OtherTerms
                }
            )
            .ToList();

            return model;
        }

        private async Task<List<SnomedTermModel>> GetSnomedTermsAsync(string wildcard)
        {
            var snomedTerms = await _biobankReadService.ListSnomedTermsAsync(wildcard);

            var model = snomedTerms.Select(x =>
               new SnomedTermModel
               {
                   SnomedTermId = x.Id,
                   Description = x.Description,
                   OtherTerms = x.OtherTerms
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
            });
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
        public ActionResult Unified(string snomedTerm, string searchRadio)
        {
            switch (searchRadio)
            {
                case "Collections":
                    return RedirectToAction("Collections", new { snomedTerm });
                case "Capabilities":
                    return RedirectToAction("Capabilities", new { snomedTerm });
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
