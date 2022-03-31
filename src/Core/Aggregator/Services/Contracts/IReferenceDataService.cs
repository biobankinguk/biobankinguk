using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface IReferenceDataService
    {
        AgeRange GetAgeRange(string age);

        CollectionPercentage GetCollectionPercentage(decimal percentage);

        CollectionStatus GetCollectionStatus(bool complete);

        DonorCount GetDonorCount(int count);

        OntologyTerm GetOntologyTerm(string id);

        AgeRange GetDefaultAgeRange();

        Task<int> GetUsageCount(int id);

        Task<ICollection<AccessCondition>> List();
       
    }
}
