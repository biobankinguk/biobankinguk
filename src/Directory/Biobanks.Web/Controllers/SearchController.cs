using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Directory.Search.Legacy;
using Directory.Search.Dto.Facets;
using Directory.Search.Constants;
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
        public async Task<ViewResult> Collections(string diagnosis, string selectedFacets)
        {
            // Build the base model.

            if (diagnosis == null)
            {
                // This is meant to be " " as opposed to "" or string.Empty, as the latter result in blank NoResults views.
                diagnosis = " ";
            }
            
            var model = new BaseSearchModel
            {
                Diagnosis = diagnosis
            };

            
            // Extract the search facets.
            model.SelectedFacets = ExtractSearchFacets(selectedFacets);

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchByDiagnosis(
                diagnosis,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    Diagnosis = model.Diagnosis,
                    SearchType = SearchDocumentType.Collection
                });

            return View(model);
        }

        private async Task<ViewResult> NoResults(NoResultsModel model)
        {
            model.Suggestions = await GetDiagnosesSearchResultsAsync(model.SearchType, model.Diagnosis?.ToLower());

            //BIO-455 special case for cancer (will override this with a genericised approach in BIO-447
            if (model.Diagnosis.ToLower() == "cancer")
            {
                //get suggestions for the relevant correct searches
                var malignant = await GetDiagnosesSearchResultsAsync(model.SearchType, "malignant");
                var neoplasm = await GetDiagnosesSearchResultsAsync(model.SearchType, "neoplasm");

                //munge them into a distinct list
                var results = new List<DiagnosisModel>();
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
        public async Task<ViewResult> CollectionsDetail(string biobankExternalId, string diagnosis, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CollectionSearchByDiagnosisAndBiobank(biobankExternalId, diagnosis, searchFacets);

            var model = _mapper.Map<DetailedCollectionSearchModel>(searchResults);

            model.Diagnosis = diagnosis;
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
        public async Task<ViewResult> Capabilities(string diagnosis, string selectedFacets)
        {
            if (diagnosis == null)
            {
                // This is meant to be " " as opposed to "" or string.Empty, as the latter result in blank NoResults views.
                diagnosis = " ";
            }

            // Build the base model.
            var model = new BaseSearchModel
            {
                Diagnosis = diagnosis
            };

            // Extract the search facets.
            model.SelectedFacets = ExtractSearchFacets(selectedFacets);

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchByDiagnosis(
                diagnosis,
                BuildSearchFacets(model.SelectedFacets),
                0);

            // Build up the rest of the model from the search results.
            _mapper.Map(searchResults, model);

            //Handle no results
            if (!model.Biobanks.Any())
                return await NoResults(new NoResultsModel
                {
                    Diagnosis = model.Diagnosis,
                    SearchType = SearchDocumentType.Collection
                });

            return View(model);
        }

        [HttpGet]
        public async Task<ViewResult> CapabilitiesDetail(string biobankExternalId, string diagnosis, string selectedFacets)
        {
            // Get the search facets.
            var searchFacets = string.IsNullOrEmpty(selectedFacets)
                ? null
                : BuildSearchFacets(ExtractSearchFacets(selectedFacets));

            // Search based on the provided criteria.
            var searchResults = _searchProvider.CapabilitySearchByDiagnosisAndBiobank(biobankExternalId, diagnosis, searchFacets);

            var model = _mapper.Map<DetailedCapabilitySearchModel>(searchResults);

            model.Diagnosis = diagnosis;
            model.SelectedFacets = selectedFacets;

            // Get the biobank logo name from the database.
            model.LogoName = (await _biobankReadService.GetBiobankByExternalIdAsync(biobankExternalId)).Logo;

            return View(model);
        }

        #region Diagnosis Type Ahead
        [AllowAnonymous]
        public async Task<JsonpResult> ListDiagnoses(string wildcard, string callback)
        {
            var diagnosisModels = await GetDiagnosesAsync(wildcard);

            return this.Jsonp(diagnosisModels, callback, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public async Task<JsonpResult> SearchDiagnoses(string searchDocumentType, string wildcard, string callback)
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

            var diagnosisModels = await GetDiagnosesSearchResultsAsync(type, wildcard.ToLower());

            return this.Jsonp(diagnosisModels, callback, JsonRequestBehavior.AllowGet);
        }

        private async Task<List<DiagnosisModel>> GetDiagnosesSearchResultsAsync(SearchDocumentType type, string wildcard)
        {
            var diagnoses = await _biobankReadService.ListSearchableDiagnosesAsync(type, wildcard);

            return diagnoses
               .Select(x => new DiagnosisModel
               {
                   Id = x.DiagnosisId,
                   SnomedIdentifier = x.SnomedIdentifier,
                   Description = x.Description,
               }).ToList();
        }

        private async Task<List<DiagnosisModel>> GetDiagnosesAsync(string wildcard)
        {
            var diagnoses = await _biobankReadService.ListDiagnosesAsync(wildcard);

            return diagnoses
               .Select(x => new DiagnosisModel
               {
                   Id = x.DiagnosisId,
                   SnomedIdentifier = x.SnomedIdentifier,
                   Description = x.Description,
               }).ToList();
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
        public ActionResult Unified(string diagnosis, string searchRadio)
        {
            switch (searchRadio)
            {
                case "Collections":
                    return RedirectToAction("Collections", new {diagnosis});
                case "Capabilities":
                    return RedirectToAction("Capabilities", new {diagnosis});
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
