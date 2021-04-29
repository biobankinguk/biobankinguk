using Biobanks.Aggregator.Core.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data.ReferenceData;
using System.Linq;
using System.Xml;

namespace Biobanks.Aggregator.Core.Services
{
    public class ReferenceDataService : IReferenceDataService
    {
        private readonly BiobanksDbContext _db;

        public ReferenceDataService(BiobanksDbContext db)
        {
            _db = db;
        }

        public AgeRange GetAgeRange(string age)
            => _db.AgeRanges.ToList().FirstOrDefault(y =>
                    XmlConvert.ToTimeSpan(y.LowerBound) <= XmlConvert.ToTimeSpan(age) &&
                    XmlConvert.ToTimeSpan(y.UpperBound) >= XmlConvert.ToTimeSpan(age)) ?? GetDefaultAgeRange();

        public CollectionPercentage GetCollectionPercentage(decimal percentage)
            => _db.CollectionPercentages.FirstOrDefault(y =>
                    y.LowerBound <= percentage &&
                    y.UpperBound >= percentage);

        public CollectionStatus GetCollectionStatus(bool complete)
        {
            return complete
                ? _db.CollectionStatus.Where(x => x.Value == "Completed").First()
                : _db.CollectionStatus.Where(x => x.Value == "In progress").First();
        }

        public DonorCount GetDonorCount(int count)
            => _db.DonorCounts.First(x => x.LowerBound <= count && x.UpperBound >= count);

        public AccessCondition GetDefaultAccessCondition()
            => _db.AccessConditions.OrderBy(x => x.SortOrder).First();

        public AgeRange GetDefaultAgeRange()
            => _db.AgeRanges.First(x => x.LowerBound == null && x.UpperBound == null);

        public CollectionPoint GetDefaultCollectionPoint()
            => _db.CollectionPoints.OrderBy(x => x.SortOrder).First();

        public CollectionType GetDefaultCollectionType()
            => _db.CollectionTypes.OrderBy(x => x.SortOrder).First();

    }
}
