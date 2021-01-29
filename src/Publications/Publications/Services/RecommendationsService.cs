using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Publications.Services.Contracts;
using Publications.Services.Dto;

namespace Publications.Services
{
    public class RecommendationsService : IRecommendationsService
    {
        private IEpmcService _epmcService;
        private IBiobankReadService _biobankReadService;
        public RecommendationsService(IEpmcService epmcService, IBiobankReadService biobankReadService)
        {
            _epmcService = epmcService;
            _biobankReadService = biobankReadService;
        }

        private async Task<JaccardIndexDTO> JaccardSimilarity(List<string> list1Original, List<string> list2Original)
        {
            //Using Hashsets as an alternative to sets in Python
            HashSet<string> hs1 = new HashSet<string>(list1Original.Select(x => x.ToLower()));
            HashSet<string> hs2 = new HashSet<string>(list2Original.Select(x => x.ToLower()));

            var intersection = hs1.Intersect(hs2);
            var union = hs1.Union(hs2);
            
            var jaccardIndex = (float)intersection.Count() / union.Count();

            //var jaccardDistance = (float)union.Count() - intersection.Count() / union.Count();
            //Can multiply jaccardIndex by 100 to give percentage
            // Alternative to pythons sorted method -> sorts intersection IEnumerable in ascending order
            intersection.OrderBy(x => x);

            var result = new JaccardIndexDTO
            {
                JaccardIndex = jaccardIndex,
                CommonAnnotations = string.Join(", ", intersection)
            };

            return result;
        }

        public async Task<List<JaccardIndexDTO>> CalculateRecommendation(string publicationId, string source)
        {
            var recommendationsList = new List<JaccardIndexDTO>();
            //Get all annotations for a specific publication
            var annotationDTO = await _epmcService.GetPublicationAnnotations(publicationId, source);

            //Put all annotations into a list of strings
            var publicationsAnnotationListA = new List<string>();
            foreach (var annotation in annotationDTO)
            {
               foreach(var tag in annotation.Tags)
                {
                    publicationsAnnotationListA.Add(tag.Name);
                }
            }

            var biobankList = await _biobankReadService.GetOrganisationIds();

            var annotationListB = new List<string>();

            foreach (var biobank in biobankList)
            {
                //Clear the annotation list for each biobank
                annotationListB.Clear();
                var publicationList = await _biobankReadService.ListOrganisationPublications(biobank);

                    foreach (var publication in publicationList)
                    {
                        foreach (var annotation in publication.PublicationAnnotations)
                        {
                            annotationListB.Add(annotation.Annotation.Name);
                        }
                    }
                    
                    //Only create a recommendation object if the biobank has publications which contain annotations
                    if (annotationListB.Count() > 0)
                    {
                        var response = await JaccardSimilarity(publicationsAnnotationListA, annotationListB);

                        var recommendation = new JaccardIndexDTO
                        {
                            OrganisationId = biobank,
                            JaccardIndex = response.JaccardIndex,
                            CommonAnnotations = response.CommonAnnotations
                        };
                        recommendationsList.Add(recommendation);
                    }

            }

            return recommendationsList.OrderByDescending(x => x.JaccardIndex).ToList();
        }

        public async Task<List<JaccardIndexDTO>> CalculateRecommendationByPublication(string publicationId, string source)
        {
            var recommendationsList = new List<JaccardIndexDTO>();
            //Get all annotations for a specific publication
            var annotationDTO = await _epmcService.GetPublicationAnnotations(publicationId, source);

            //Put all annotations for given publication into a list of strings
            var publicationsAnnotationListA = new List<string>();
            foreach (var annotation in annotationDTO)
            {
                foreach (var tag in annotation.Tags)
                {
                    publicationsAnnotationListA.Add(tag.Name);
                }
            }

            //Get all publications from directory
            var publications = await _biobankReadService.ListPublications();
            var annotationListB = new List<string>();
            foreach (var publication in publications)
            {
                annotationListB.Clear();
                foreach (var annotation in publication.PublicationAnnotations)
                {
                    annotationListB.Add(annotation.Annotation.Name);
                }


                //Only create a recommendation object if the biobank has publications which contain annotations
                if (annotationListB.Count() > 0)
                {
                    var response = await JaccardSimilarity(publicationsAnnotationListA, annotationListB);

                    var recommendation = new JaccardIndexDTO
                    {
                        OrganisationId = publication.OrganisationId,
                        JaccardIndex = response.JaccardIndex,
                        CommonAnnotations = response.CommonAnnotations,
                        Publication = publication
                    };
                    recommendationsList.Add(recommendation);
                }

            }

            var uniqBiobank = new HashSet<int>();
            var uniqResult = new List<JaccardIndexDTO>();

            var result = recommendationsList.OrderByDescending(x => x.JaccardIndex).ToList();

            foreach (var p in result)
            {
                if (!uniqBiobank.Contains(p.OrganisationId))
                {
                    uniqBiobank.Add(p.OrganisationId);
                    uniqResult.Add(p);
                }
            }


            return uniqResult;
        }

    }
}
