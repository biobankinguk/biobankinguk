﻿using Biobanks.Entities.Data.ReferenceData;


namespace Biobanks.Aggregator.Core.Services.Contracts
{
    public interface IReferenceDataService
    {
        AgeRange GetAgeRange(string age);

        CollectionPercentage GetCollectionPercentage(decimal percentage);

        CollectionStatus GetCollectionStatus(bool complete);

        DonorCount GetDonorCount(int count);

        AccessCondition GetDefaultAccessCondition();

        AgeRange GetDefaultAgeRange();

        CollectionPoint GetDefaultCollectionPoint();

        CollectionType GetDefaultCollectionType();

        HtaStatus GetDefaultHtaStatus();
    }
}
