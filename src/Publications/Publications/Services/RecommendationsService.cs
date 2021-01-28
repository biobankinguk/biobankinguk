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

        public RecommendationsService()
        {

        }

        private async Task<JaccardIndexDTO> JaccardSimilarity(List<string> list1Original, List<string> list2Original)
        {
            //Using Hashsets as an alternative to sets in Python
            HashSet<string> hs1 = new HashSet<string>(list1Original.Select(x => x.ToLower()));
            HashSet<string> hs2 = new HashSet<string>(list2Original.Select(x => x.ToLower()));

            var intersection = hs1.Intersect(hs2);
            var union = hs1.Union(hs2);

            var jaccardDistance = (float)union.Count() - intersection.Count() / union.Count();

            // Alternative to pythons sorted method -> sorts intersection IEnumerable in ascending order
            intersection.OrderBy(x => x);

            var result = new JaccardIndexDTO
            {
                JaccardIndex = 1.0 - jaccardDistance,
                CommonAnnotations = string.Join(", ", intersection)
            };

            return result;
        }

        

    }
}
