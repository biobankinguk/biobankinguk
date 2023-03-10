using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Services;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class BiobankWriteService : IBiobankWriteService
    {
        #region Properties and ctor
        
        private readonly IBiobankIndexService _indexService;
        private readonly ILogoStorageProvider _logoStorageProvider;
        private readonly ApplicationDbContext _context;

    public BiobankWriteService(
            IBiobankIndexService indexService,
            ILogoStorageProvider logoStorageProvider,
            IGenericEFRepository<Funder> funderRepository,
            ApplicationDbContext context)
        {
            _indexService = indexService;
            _logoStorageProvider = logoStorageProvider;
            _context = context;
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
                if (await _context.FindAsync<ServiceOffering>(service.ServiceOfferingId) != null)
                {

               //now make sure the biobank doesn't already have this service listed
                var biobank = await _context.OrganisationServiceOfferings
                .AsNoTracking()
                .Where(x => x.OrganisationId == service.OrganisationId && x.ServiceOfferingId == service.ServiceOfferingId)
                .ToListAsync();

                if(biobank.FirstOrDefault() == null)
                  {
                    _context.Add(service);
                  }
                }
                //atm we just silently fail if the service id is invalid; should we be throwing?
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBiobankServiceAsync(int biobankId, int serviceId)
        {
            //make sure the biobank has this service

               var biobank = await _context.OrganisationServiceOfferings
                .AsNoTracking()
                .Where(x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId)
                .ToListAsync();

              var entity = _context.OrganisationServiceOfferings.Where(x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId).FirstOrDefault();

                if(biobank.FirstOrDefault() != null)
                  {
                    _context.OrganisationServiceOfferings.Remove(entity);
                  }

              await _context.SaveChangesAsync();
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

            var organistaionAnnualStats = await _context.OrganisationAnnualStatistics
            .AsNoTracking()
            .Where(x => x.OrganisationId == organisationId && x.AnnualStatisticId == statisticId && x.Year == year)
            .ToListAsync();

           var existingEntry = organistaionAnnualStats.FirstOrDefault();

            if (existingEntry != null)
            {
                existingEntry.Value = value;
                _context.OrganisationAnnualStatistics.Update(existingEntry);
            }

            else
                _context.OrganisationAnnualStatistics.Update(organisationAnnualStatistic);

            await _context.SaveChangesAsync();
        }

        public async Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons)
        {
            foreach (var registrationReason in activeRegistrationReasons)
            {
                //Validate reason id first - don't want to go around inserting new unnamed reasons
                if (await _context.OrganisationRegistrationReasons.FindAsync(registrationReason.RegistrationReasonId) != null)
                {
                    //now make sure the biobank doesn't already have this reason listed
                    var biobank = await _context.OrganisationRegistrationReasons
                    .Where(x => x.OrganisationId == registrationReason.OrganisationId && x.RegistrationReasonId == registrationReason.RegistrationReasonId)
                    .ToListAsync();

                    if (biobank.FirstOrDefault() == null) 
                    {
                        _context.OrganisationRegistrationReasons.Add(registrationReason);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId)
        {
            //make sure the biobank has this reason
            var biobank = await _context.OrganisationRegistrationReasons
            .Where(x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId)
            .ToListAsync();

            var entity = _context.OrganisationRegistrationReasons.Where(x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId).FirstOrDefault();

          if (biobank.FirstOrDefault() != null) 
            {
             _context.OrganisationRegistrationReasons.Remove(entity);
            };

            await _context.SaveChangesAsync();
        }

    }
}
