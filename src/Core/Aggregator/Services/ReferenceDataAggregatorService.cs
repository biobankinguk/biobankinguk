﻿using Biobanks.Aggregator.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared.ReferenceData;
using Biobanks.Submissions.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Aggregator.Services
{
    public class ReferenceDataAggregatorService : IReferenceDataAggregatorService
    {
        private readonly BiobanksDbContext _db;

        public ReferenceDataAggregatorService(BiobanksDbContext db)
        {
            _db = db;
        }

        public AgeRange GetAgeRange(string age)
            => _db.AgeRanges.ToList().FirstOrDefault(y => y.ContainsTimeSpan(age)) ?? GetDefaultAgeRange();

        public CollectionPercentage GetCollectionPercentage(decimal percentage)
            => _db.CollectionPercentages.FirstOrDefault(y =>
                    y.LowerBound <= percentage &&
                    y.UpperBound >= percentage);

        public CollectionStatus GetCollectionStatus(bool complete)
            => complete
                ? _db.CollectionStatus.Where(x => x.Value == "Completed").First()
                : _db.CollectionStatus.Where(x => x.Value == "In progress").First();

        public DonorCount GetDonorCount(int count)
            => _db.DonorCounts.First(x => x.LowerBound <= count && x.UpperBound >= count);

        public OntologyTerm GetOntologyTerm(string id)
            => _db.OntologyTerms.FirstOrDefault(x => x.Id == id);

        public AgeRange GetDefaultAgeRange()
            => _db.AgeRanges.First(x => x.LowerBound == null && x.UpperBound == null);

    }
}
