using Publications.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Publications.Services.Contracts
{
    public interface IRecommendationsService
    {
        Task<List<JaccardIndexDTO>> CalculateRecommendation(string publicationId, string source);

        Task<List<JaccardIndexDTO>> CalculateRecommendationByPublication(string publicationId, string source);
    }
}
