using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Data.Entities.ReferenceData;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Submissions;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class BiobankWriteService : IBiobankWriteService
    {
        #region Properties and ctor
        
        private readonly ILogoStorageProvider _logoStorageProvider;
        private readonly ApplicationDbContext _db;

    public BiobankWriteService(
            ILogoStorageProvider logoStorageProvider,
            ApplicationDbContext db)
            {
                _logoStorageProvider = logoStorageProvider;
                _db = db;
            }

        #endregion
       
         public async Task<string> StoreLogoAsync(Stream logoFileStream, string logoFileName, string logoContentType, string reference)
         {
              var resizedLogoStream = await ImageService.ResizeImageStream(logoFileStream, maxX: 300, maxY: 300);

              return await _logoStorageProvider.StoreLogoAsync(resizedLogoStream, logoFileName, logoContentType, reference);
         }

        public async Task RemoveLogoAsync(int organisationId)
        {
            await _logoStorageProvider.RemoveLogoAsync(organisationId);
        }

        public async Task AddBiobankServicesAsync(IEnumerable<OrganisationServiceOffering> services)
        {
            foreach (var service in services)
            {
                //Validate service id first - don't want to go around inserting new unnamed services
                if (await _db.FindAsync<ServiceOffering>(service.ServiceOfferingId) != null)
                {

               //now make sure the biobank doesn't already have this service listed
                var biobank = await _db.OrganisationServiceOfferings
                .AsNoTracking()
                .Where(x => x.OrganisationId == service.OrganisationId && x.ServiceOfferingId == service.ServiceOfferingId)
                .ToListAsync();

                if(biobank.FirstOrDefault() == null)
                  {
                    _db.Add(service);
                  }
                }
                //atm we just silently fail if the service id is invalid; should we be throwing?
            }
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBiobankServiceAsync(int biobankId, int serviceId)
        {
            //make sure the biobank has this service

               var biobank = await _db.OrganisationServiceOfferings
                .AsNoTracking()
                .Where(x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId)
                .ToListAsync();

              var entity = _db.OrganisationServiceOfferings.Where(x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId).FirstOrDefault();

                if(biobank.FirstOrDefault() != null)
                  {
                    _db.OrganisationServiceOfferings.Remove(entity);
                  }

              await _db.SaveChangesAsync();
        }


        public async Task UpdateOrganisationAnnualStatisticAsync(int organisationId, int statisticId, int? value, int year)
        {
            var organisationAnnualStatistic = new OrganisationAnnualStatistic
            {
                OrganisationId = organisationId,
                AnnualStatisticId = statisticId,
                Value = value,
                Year = year
            };

            var organistaionAnnualStats = await _db.OrganisationAnnualStatistics
            .AsNoTracking()
            .Where(x => x.OrganisationId == organisationId && x.AnnualStatisticId == statisticId && x.Year == year)
            .ToListAsync();

           var existingEntry = organistaionAnnualStats.FirstOrDefault();

            if (existingEntry != null)
            {
                existingEntry.Value = value;
                _db.OrganisationAnnualStatistics.Update(existingEntry);
            }

            else
                _db.OrganisationAnnualStatistics.Update(organisationAnnualStatistic);

            await _db.SaveChangesAsync();
        }

        public async Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons)
        {
            foreach (var registrationReason in activeRegistrationReasons)
            {
                //Validate reason id first - don't want to go around inserting new unnamed reasons
                if (await _db.RegistrationReasons.FindAsync(registrationReason.RegistrationReasonId) != null)
                {
                    //now make sure the biobank doesn't already have this reason listed
                    var biobank = await _db.OrganisationRegistrationReasons
                    .Where(x => x.OrganisationId == registrationReason.OrganisationId && x.RegistrationReasonId == registrationReason.RegistrationReasonId)
                    .ToListAsync();

                    if (biobank.FirstOrDefault() == null) 
                    {
                        _db.OrganisationRegistrationReasons.Add(registrationReason);
                    }
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId)
        {
            //make sure the biobank has this reason
            var biobank = await _db.OrganisationRegistrationReasons
            .Where(x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId)
            .ToListAsync();

            var entity = _db.OrganisationRegistrationReasons.Where(x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId).FirstOrDefault();

          if (biobank.FirstOrDefault() != null) 
            {
             _db.OrganisationRegistrationReasons.Remove(entity);
            };

            await _db.SaveChangesAsync();
        }

    }
}
