using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Data.Entities.Shared.ReferenceData;

namespace Biobanks.Aggregator.Services.Contracts
{
    public interface IReferenceDataAggregatorService
    {
        AgeRange GetAgeRange(string age);

        CollectionPercentage GetCollectionPercentage(decimal percentage);

        CollectionStatus GetCollectionStatus(bool complete);

        DonorCount GetDonorCount(int count);

        OntologyTerm GetOntologyTerm(string id);

        AgeRange GetDefaultAgeRange();
       
    }
}
