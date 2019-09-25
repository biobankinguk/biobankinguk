using Common.Data.ReferenceData;
using Microsoft.EntityFrameworkCore;

namespace Common.Data
{
    public class DirectoryContext : DbContext
    {

        public DirectoryContext(DbContextOptions<DirectoryContext> options)
            : base(options)
        {

        }

        public DbSet<AccessCondition> AccessConditions { get; set; }
        public DbSet<AgeRange> AgeRanges { get; set; }
        public DbSet<AnnualStatistic> AnnualStatistics { get; set; }
        public DbSet<AssociatedDataProcurementTimeframe> AssociatedDataProcurementTimeframes { get; set; }
        public DbSet<AssociatedDataType> AssociatedDataTypes { get; set; }
        public DbSet<CollectionPercentage> CollectionPercentages { get; set; }
        public DbSet<CollectionPoint> CollectionPoints { get; set; }
        public DbSet<CollectionStatus> CollectionStatuses { get; set; }
        public DbSet<CollectionType> CollectionTypes { get; set; }
        public DbSet<ConsentRestriction> ConsentRestrictions { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<DonorCount> DonorCounts { get; set; }
        public DbSet<Funder> Funders { get; set; }
        public DbSet<HtaStatus> HtaStatuses { get; set; }
        public DbSet<MacroscopicAssessment> MacroscopicAssessments { get; set; }
        public DbSet<MaterialType> MaterialTypes { get; set; }
        public DbSet<OntologyTerm> OntologyTerms { get; set; }
        public DbSet<ServiceOffering> ServiceOfferings { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<SopStatus> SopStatuses { get; set; }
        public DbSet<StorageTemperature> StorageTemperatures { get; set; }
    }
}
