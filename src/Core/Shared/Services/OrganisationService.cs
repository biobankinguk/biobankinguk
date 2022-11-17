using Biobanks.Shared.Services.Contracts;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Hangfire;

namespace Biobanks.Shared.Services
{
    //TODO merge or resolve with OrganisationDirectoryService
    public class OrganisationService : IOrganisationService
    {
        private readonly BiobanksDbContext _db;

        public OrganisationService(BiobanksDbContext db)
        {
            _db = db;
        }

        public async Task<int> Count()
            => await _db.Organisations.CountAsync();

        public async Task<IEnumerable<Organisation>> List()
            => await _db.Organisations.ToListAsync();

        public async Task<IEnumerable<string>> ListExternalIds()
            => await _db.Organisations.Select(x => x.OrganisationExternalId).ToListAsync();

        public async Task<Organisation> GetById(int organisationId)
            => await _db.Organisations
                .Include(x => x.AccessCondition)
                .Include(x => x.CollectionType)
                .FirstOrDefaultAsync(x => x.OrganisationId == organisationId);
    public async Task<Organisation> Update(Organisation organisation)
    {
      var currentOrganisation = await _db.Organisations
          .FirstOrDefaultAsync(x => x.OrganisationId == organisation.OrganisationId);

      if (currentOrganisation != null)
      {
        currentOrganisation.ExcludePublications = organisation.ExcludePublications;
        currentOrganisation.Name = organisation.Name;
        currentOrganisation.Description = organisation.Description;
        currentOrganisation.Url = organisation.Url;
        currentOrganisation.ContactEmail = organisation.ContactEmail;
        currentOrganisation.ContactNumber = organisation.ContactNumber;
        currentOrganisation.AddressLine1 = organisation.AddressLine1;
        currentOrganisation.AddressLine2 = organisation.AddressLine2;
        currentOrganisation.AddressLine3 = organisation.AddressLine3;
        currentOrganisation.AddressLine4 = organisation.AddressLine4;
        currentOrganisation.Logo = organisation.Logo;
        currentOrganisation.City = organisation.City;
        currentOrganisation.CountyId = organisation.CountyId;
        currentOrganisation.CountryId = organisation.CountryId;
        currentOrganisation.PostCode = organisation.PostCode;
        currentOrganisation.GoverningInstitution = organisation.GoverningInstitution;
        currentOrganisation.GoverningDepartment = organisation.GoverningDepartment;
        currentOrganisation.EthicsRegistration = organisation.EthicsRegistration;
        currentOrganisation.SharingOptOut = organisation.SharingOptOut;
        currentOrganisation.IsSuspended = organisation.IsSuspended;
        currentOrganisation.OtherRegistrationReason = organisation.OtherRegistrationReason;
        currentOrganisation.AccessConditionId = organisation.AccessConditionId;
        currentOrganisation.CollectionTypeId = organisation.CollectionTypeId;

        //need to clear and load these collections due to tracking
        currentOrganisation.OrganisationRegistrationReasons?.Clear();
        currentOrganisation.OrganisationServiceOfferings?.Clear();


        var allRegistrationReasons = await _db.OrganisationRegistrationReasons.ToListAsync();
        foreach (var orr in organisation.OrganisationRegistrationReasons)
        {
          var orgReason = allRegistrationReasons.SingleOrDefault(x => x.OrganisationId == orr.OrganisationId
              && x.RegistrationReasonId == orr.RegistrationReasonId);
          currentOrganisation.OrganisationRegistrationReasons.Add(orgReason);
        }

        var allServiceOfferings = await _db.OrganisationServiceOfferings.ToListAsync();
        foreach (var ser in organisation.OrganisationServiceOfferings)
        {
          var serviceOffering = allServiceOfferings.SingleOrDefault(x => x.OrganisationId == ser.ServiceOfferingId
              && x.ServiceOfferingId == ser.ServiceOfferingId);
        }

        // Set Timestamp
        currentOrganisation.LastUpdated = DateTime.Now;

        // Ensure AnonymousIdentifier Exists
        if (!currentOrganisation.AnonymousIdentifier.HasValue)
          currentOrganisation.AnonymousIdentifier = Guid.NewGuid();

        await _db.SaveChangesAsync();

        // Update Organisation Index
        if (!await IsSuspended(currentOrganisation.OrganisationId))
        {
          var organisationServiceOfferings = await _db.OrganisationServiceOfferings.AsNoTracking()
              .Where(x => x.OrganisationId == currentOrganisation.OrganisationId)
              .ToListAsync();

          var partial = new PartialBiobank
          {
            Biobank = currentOrganisation.Name,
            BiobankServices = organisationServiceOfferings.Select(x => new BiobankServiceDocument
            {
              Name = x.ServiceOffering.Value
            })
          };

          // Update Collections
          currentOrganisation.Collections
              .SelectMany(c => c.SampleSets)
              .ToList()
              .ForEach(s => BackgroundJob.Enqueue(() => _collectionsIndex.Update(s.Id, partial)));

          // Update Capabilities
          currentOrganisation.DiagnosisCapabilities
              .ToList()
              .ForEach(c => BackgroundJob.Enqueue(() => _capabilitiesIndex.Update(c.DiagnosisCapabilityId, partial)));
        }

      }
      return organisation;
    }
    public async Task<bool> IsSuspended(int organisationId)
           => await _db.Organisations.AnyAsync(x => x.OrganisationId == organisationId && x.IsSuspended);

  }
}
