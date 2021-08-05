using Biobanks.Directory.Data;
using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Entities.Data;
using Biobanks.Entities.Data.ReferenceData;
using Biobanks.Entities.Shared;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Extensions;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations.Model;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly BiobanksDbContext _db;

        public OrganisationService(BiobanksDbContext db)
        {
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<OrganisationType> GetBiobankOrganisationTypeAsync()
            => await _db.OrganisationTypes.FirstOrDefaultAsync(x => x.Description == "Biobank");

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> List(string name = "", bool includeSuspended = true)
            => await _db.Organisations
                .Where(x => x.Name.Contains(name) && (includeSuspended || x.IsSuspended == false))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByExternalIds(IList<string> externalIds)
            => await _db.Organisations
                .Include(x => x.OrganisationNetworks)
                .Where(x => externalIds.Contains(x.OrganisationExternalId))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByAnonymousIdentifiers(IEnumerable<Guid> identifiers)
            => await _db.Organisations
                .Where(x => x.AnonymousIdentifier.HasValue && identifiers.Contains(x.AnonymousIdentifier.Value))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<Organisation> Get(int id)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return await _db.Organisations
                .Include(x => x.ApiClients)
                .Include(x => x.Funders)
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.OrganisationId == id && x.OrganisationTypeId == type.OrganisationTypeId);
        }
        
        /// <inheritdoc/>
        public async Task<Organisation> GetForIndexing(int id)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return await _db.Organisations
                .Include(x => x.Collections)
                .Include(x => x.Collections.Select(c => c.SampleSets))
                .Include(x => x.DiagnosisCapabilities)
                .Include(x => x.OrganisationServiceOfferings)
                .Include(x => x.OrganisationServiceOfferings.Select(o => o.ServiceOffering))
                .FirstOrDefaultAsync(x => x.OrganisationId == id && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetByName(string name)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return await _db.Organisations
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.Name == name && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetByExternalId(string externalId)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return await _db.Organisations
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.OrganisationExternalId == externalId && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetByExternalIdForSearch(string externalId)
        {
            var type = await GetBiobankOrganisationTypeAsync();
            return await _db.Organisations
                .Include(x => x.DiagnosisCapabilities)
                .Include(x => x.DiagnosisCapabilities.Select(c => c.SampleCollectionMode))
                .Include(x => x.DiagnosisCapabilities.Select(c => c.AssociatedData))
                .Include(x => x.DiagnosisCapabilities.Select(c => c.AssociatedData.Select(ad => ad.AssociatedDataType)))
                .Include(x => x.DiagnosisCapabilities.Select(c => c.AssociatedData.Select(ad => ad.AssociatedDataProcurementTimeframe)))
                .Include(x => x.OrganisationServiceOfferings)
                .Include(x => x.OrganisationServiceOfferings.Select(o => o.ServiceOffering))
                .FirstOrDefaultAsync(x => x.OrganisationExternalId == externalId && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> Create(Organisation organisation)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            // TODO: Support ISO3 Country Codes
            organisation.OrganisationExternalId = "GBR-" + type.OrganisationTypeId + "-";
            organisation.OrganisationTypeId = type.OrganisationTypeId;

            organisation.AnonymousIdentifier = Guid.NewGuid();
            organisation.LastUpdated = DateTime.Now;

            _db.Organisations.Add(organisation);

            // TODO: Is this check relevant?
            if (await _db.SaveChangesAsync() != 1)
                throw new DataException();

            // TODO: Is there a better External ID Schema
            // Update External Id
            organisation.OrganisationExternalId += organisation.OrganisationId;

            //await UpdateBiobankAsync(organisation);

            return organisation;
        }

        /// <inheritdoc/>
        public async Task<Organisation> Update(Organisation organisation)
        {
            var biobank = await Get(organisation.OrganisationId);

            // Update Timestamp
            biobank.LastUpdated = DateTime.Now;

            // TODO: Map Fields
            biobank.Url = UrlTransformer.Transform(organisation.Url);

            // Ensure Organisation Has An Anonymous Identifier
            if (!biobank.AnonymousIdentifier.HasValue)
                biobank.AnonymousIdentifier = Guid.NewGuid();

            // TODO: Solve circular dependency
            //if (!await IsBiobankSuspendedAsync(biobank.OrganisationId))
                //await _indexService.UpdateBiobankDetails(biobank.OrganisationId);

            return biobank;
        }

        /// <inheritdoc/>
        public async Task Delete(int id)
        {
            // TODO: Solve this circular dependency
            //await _indexService.BulkDeleteBiobank(id);

            var organisation = new Organisation { OrganisationId = id };
            _db.Organisations.Attach(organisation);
            _db.Organisations.Remove(organisation);

            await _db.SaveChangesAsync();
        }
        
        /// <inheritdoc/>
        public async Task<OrganisationUser> AddUser(string userId, int organisationId)
        {
            // TODO: Do we need any validation here?
            var user = new OrganisationUser
            {
                OrganisationId = organisationId,
                OrganisationUserId = userId
            };

            _db.OrganisationUsers.Add(user);

            await _db.SaveChangesAsync();

            return user;
        }

        /// <inheritdoc/>
        public async Task RemoveUser(string userId, int organisationId)
        {
            var user = new OrganisationUser
            {
                OrganisationId = organisationId,
                OrganisationUserId = userId
            };

            _db.OrganisationUsers.Attach(user);
            _db.OrganisationUsers.Remove(user);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> AddFunder(int funderId, int organisationId)
        {
            var organisation = await Get(organisationId);
            
            // TODO: User FunderService?
            var funder = await _db.Funders.FirstOrDefaultAsync(x => x.Id == funderId); 

            if (organisation is null || funder is null)
                return false;

            // Add Organisation To Funder
            organisation.Funders.Add(funder);

            await _db.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task RemoveFunder(int funderId, int organisationId)
        {
            var organisation = await Get(organisationId);

            organisation.Funders.Remove(
                organisation.Funders.FirstOrDefault(x => x.Id == funderId));

            await Update(organisation);
        }

        /// <inheritdoc/>
        public async Task<Organisation> Suspend(int organisationId)
        {
            var organisation = await Get(organisationId);

            organisation.IsSuspended = true;

            return await Update(organisation);
        }

        /// <inheritdoc/>
        public async Task<Organisation> Unsuspend(int organisationId)
        {
            var organisation = await Get(organisationId);

            organisation.IsSuspended = false;

            return await Update(organisation);
        }

        /// <inheritdoc/>
        public async Task<bool> IsSuspended(int organisationId)
            => await _db.Organisations.AnyAsync(x => x.OrganisationId == organisationId && x.IsSuspended);

        // TODO: Figure out a better name
        /// <inheritdoc/>
        public async Task<bool> IsApiClient(int organisationId)
            => await _db.ApiClients.AnyAsync(x => x.Organisations.Any(y => y.OrganisationId == organisationId));

        /// <inheritdoc/>
        public async Task<bool> RegistrationRequestExists(string name)
        {
            var type = await GetBiobankOrganisationTypeAsync();

            return await _db.OrganisationRegisterRequests
                .AnyAsync(x =>
                    x.OrganisationTypeId == type.OrganisationTypeId &&
                    x.OrganisationName == name && 
                    x.DeclinedDate == null);
        }

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request)
        {
            _db.OrganisationRegisterRequests.Add(request);

            await _db.SaveChangesAsync();

            return request;
        }

        /// <inheritdoc/>
        public async Task RemoveRegistrationRequest(OrganisationRegisterRequest request)
        {
            _db.OrganisationRegisterRequests.Attach(request);
            _db.OrganisationRegisterRequests.Remove(request);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<KeyValuePair<string, string>> GenerateNewApiClient(int organisationId, string clientName = null)
        {
            var clientId = Crypto.GenerateId();
            var clientSecret = Crypto.GenerateId();
            var organisation = await Get(organisationId);

            organisation.ApiClients.Add(new ApiClient
            {
                Name = clientName ?? clientId,
                ClientId = clientId,
                ClientSecretHash = clientSecret.Sha256()
            });

            await Update(organisation);

            return new KeyValuePair<string, string>(clientId, clientSecret);
        }

        /// <inheritdoc/>
        public async Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int organisationId)
        {
            var organisation = await Get(organisationId);

            var credentials = organisation.ApiClients.First();
            var newSecret = Crypto.GenerateId();

            // Update Credentials Secret
            organisation.ApiClients.First().ClientSecretHash = newSecret.Sha256();

            await Update(organisation);

            return new KeyValuePair<string, string>(credentials.ClientId, newSecret);
        }



        public List<KeyValuePair<int, string>> GetAcceptedBiobankRequestIdsAndNamesByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public List<KeyValuePair<int, string>> GetBiobankIdsAndNamesByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationRegisterRequest> GetBiobankRegisterRequestByUserEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<BiobankActivityDTO>> GetBiobanksActivityAsync()
        {
            throw new NotImplementedException();
        }


        public Task<bool> IsCapabilityBiobankSuspendedAsync(int capabilityId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCollectionBiobankSuspendedAsync(int collectonId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsSampleSetBiobankSuspendedAsync(int sampleSetId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegistrationReason>> ListBiobankRegistrationReasonsAsync(int organisationId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<OrganisationRegisterRequest>> ListOpenBiobankRegisterRequestsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<OrganisationRegisterRequest> UpdateOrganisationRegisterRequestAsync(OrganisationRegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
