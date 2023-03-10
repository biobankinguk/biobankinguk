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
            if ((await _organisationServiceOfferingRepository.ListAsync(false,
                x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId))
                .FirstOrDefault() != null)
            {
                await
                _organisationServiceOfferingRepository.DeleteWhereAsync(
                    x => x.OrganisationId == biobankId && x.ServiceOfferingId == serviceId);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddFunderToBiobankAsync(int funderId, int biobankId)
        {
            var funder = await _funderRepository.GetByIdAsync(funderId);
            var bb = await _organisationRepository.GetByIdAsync(biobankId);

            if (bb == null || funder == null) throw new ApplicationException();

            try
            {
                funder.Organisations.Add(bb);

                _funderRepository.Update(funder);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public async Task RemoveFunderFromBiobankAsync(int funderId, int biobankId)
        {
            var funder = await _funderRepository.GetByIdAsync(funderId);

            if (funder == null) throw new ApplicationException();

            funder.Organisations.Remove(
                funder.Organisations
                    .FirstOrDefault(x => x.OrganisationId == biobankId));

            _funderRepository.Update(funder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBiobankAsync(int id)
        {
            var organisation = await _organisationRepository.GetByIdAsync(id);

            // Remove From Index
            _indexService.BulkDeleteBiobank(organisation);

            // Delete Organisation
            await _organisationRepository.DeleteAsync(id);
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

            var existingEntry = (await _organisationAnnualStatisticRepository.ListAsync(true,
                oas => oas.OrganisationId == organisationId
                       && oas.AnnualStatisticId == statisticId
                       && oas.Year == year)).FirstOrDefault();

            if (existingEntry != null)
            {
                existingEntry.Value = value;
                _organisationAnnualStatisticRepository.Update(existingEntry);
            }

            else
                _organisationAnnualStatisticRepository.Insert(organisationAnnualStatistic);

            await _context.SaveChangesAsync();
        }

        public async Task AddBiobankRegistrationReasons(List<OrganisationRegistrationReason> activeRegistrationReasons)
        {
            foreach (var registrationReason in activeRegistrationReasons)
            {
                //Validate reason id first - don't want to go around inserting new unnamed reasons
                if (await _registrationReasonRepository.GetByIdAsync(registrationReason.RegistrationReasonId) != null)
                {
                    //now make sure the biobank doesn't already have this reason listed
                    if ((await _organisationRegistrationReasonRepository.ListAsync(false,
                        x => x.OrganisationId == registrationReason.OrganisationId && x.RegistrationReasonId == registrationReason.RegistrationReasonId))
                        .FirstOrDefault() == null)
                    {
                        _organisationRegistrationReasonRepository.Insert(registrationReason);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBiobankRegistrationReasonAsync(int organisationId, int registrationReasonId)
        {
            //make sure the biobank has this reason
            if ((await _organisationRegistrationReasonRepository.ListAsync(false,
                x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId))
                .FirstOrDefault() != null)
            {
                await
                _organisationRegistrationReasonRepository.DeleteWhereAsync(
                    x => x.OrganisationId == organisationId && x.RegistrationReasonId == registrationReasonId);
            }

            await _context.SaveChangesAsync();
        }

    }
}
