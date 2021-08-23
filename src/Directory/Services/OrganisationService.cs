using Biobanks.Directory.Data;
using Biobanks.Directory.Data.Transforms.Url;
using Biobanks.Entities.Data;
using Biobanks.Entities.Shared;
using Biobanks.IdentityModel.Helpers;
using Biobanks.IdentityModel.Extensions;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Identity.Data.Entities;
using Biobanks.Identity.Contracts;
using Microsoft.AspNet.Identity;
using AutoMapper;

namespace Biobanks.Directory.Services
{
    public class OrganisationService : IOrganisationService
    {
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly IBiobankIndexService _indexService;
        private readonly BiobanksDbContext _db;

        private readonly IMapper _mapper;

        public OrganisationService(
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IBiobankIndexService indexService,
            BiobanksDbContext db,
            IMapper mapper)
        {
            _userManager = userManager;
            _indexService = indexService;
            _db = db;

            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<OrganisationType> GetOrganisationType()
            => await _db.OrganisationTypes.FirstOrDefaultAsync(x => x.Description == "Biobank");

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> List(string name = "", bool includeSuspended = true)
            => await _db.Organisations
                .Where(x => x.Name.Contains(name) && (includeSuspended || x.IsSuspended == false))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListForActivity(string name = "", bool includeSuspended = true)
            => await _db.Organisations
                .AsNoTracking()
                .Include(x => x.Collections)
                .Include(x => x.DiagnosisCapabilities)
                .Include(x => x.OrganisationUsers)
                .Where(x => x.Name.Contains(name) && (includeSuspended || x.IsSuspended == false))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByNetworkId(int networkId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Where(x => x.NetworkId == networkId && !x.Organisation.IsSuspended)
                .Select(x => x.Organisation)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByNetworkIdForIndexing(int networkId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Include(x => x.Organisation.Collections)
                .Include(x => x.Organisation.Collections.Select(c => c.SampleSets))
                .Include(x => x.Organisation.DiagnosisCapabilities)
                .Where(x => x.NetworkId == networkId && !x.Organisation.IsSuspended)
                .Select(x => x.Organisation)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByUserId(string userId)
            => await _db.OrganisationUsers
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Where(x => x.OrganisationUserId == userId)
                .Select(x => x.Organisation)
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
            var type = await GetOrganisationType();

            return await _db.Organisations
                .Include(x => x.ApiClients)
                .Include(x => x.Funders)
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.OrganisationId == id && x.OrganisationTypeId == type.OrganisationTypeId);
        }
        
        /// <inheritdoc/>
        public async Task<Organisation> GetForIndexing(int id)
        {
            var type = await GetOrganisationType();

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
            var type = await GetOrganisationType();

            return await _db.Organisations
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.Name == name && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetByExternalId(string externalId)
        {
            var type = await GetOrganisationType();

            return await _db.Organisations
                .Include(x => x.OrganisationAnnualStatistics)
                .FirstOrDefaultAsync(x => x.OrganisationExternalId == externalId && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<Organisation> GetByExternalIdForSearch(string externalId)
        {
            var type = await GetOrganisationType();
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
            var type = await GetOrganisationType();

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

            await Update(organisation);

            return organisation;
        }

        /// <inheritdoc/>
        public async Task<Organisation> Update(Organisation organisation)
        {
            var existingOrganisation = await Get(organisation.OrganisationId);

            // Map Most Fields Automatically
            _mapper.Map(organisation, existingOrganisation);

            // Update Timestamp
            existingOrganisation.LastUpdated = DateTime.Now;

            // Enforce Proper URL Syntax
            existingOrganisation.Url = UrlTransformer.Transform(organisation.Url);

            // FK Can Be Referenced By Entity Instance or Directly
            existingOrganisation.AccessConditionId = organisation.AccessCondition?.Id ?? organisation.AccessConditionId;
            existingOrganisation.CollectionTypeId = organisation.CollectionType?.Id ?? organisation.CollectionTypeId;
            existingOrganisation.CountryId = organisation.Country?.Id ?? organisation.CountryId;
            existingOrganisation.CountyId = organisation.County?.Id ?? organisation.CountyId;
            existingOrganisation.OrganisationTypeId = organisation.OrganisationType?.OrganisationTypeId ?? organisation.OrganisationTypeId;

            // Ensure Organisation Has An Anonymous Identifier
            if (!existingOrganisation.AnonymousIdentifier.HasValue)
                existingOrganisation.AnonymousIdentifier = Guid.NewGuid();

            if (!await IsSuspended(organisation.OrganisationId))
                await _indexService.UpdateBiobankDetails(organisation.OrganisationId);

            return existingOrganisation;
        }

        /// <inheritdoc/>
        public async Task Delete(int id)
        {
            await _indexService.BulkDeleteBiobank(id);

            var organisation = new Organisation { OrganisationId = id };
            _db.Organisations.Attach(organisation);
            _db.Organisations.Remove(organisation);

            await _db.SaveChangesAsync();
        }
        
        /// <inheritdoc/>
        public async Task<OrganisationUser> AddUserToOrganisation(string userId, int organisationId)
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
        public async Task RemoveUserFromOrganisation(string userId, int organisationId)
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
        public async Task<bool> AddFunderToOrganisation(int funderId, int organisationId)
        {
            var organisation = await Get(organisationId);
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

        /// <inheritdoc/>
        public async Task<bool> IsSuspendedByCapability(int capabilityId)
            => await _db.DiagnosisCapabilities
                .Include(x => x.Organisation)
                .AnyAsync(x => x.DiagnosisCapabilityId == capabilityId && x.Organisation.IsSuspended);

        /// <inheritdoc/>
        public async Task<bool> IsSuspendedByCollection(int collectonId)
            => await _db.Collections
                .Include(x => x.Organisation)
                .AnyAsync(x => x.CollectionId == collectonId && x.Organisation.IsSuspended);

        /// <inheritdoc/>
        public async Task<bool> IsSuspendedBySampleSet(int sampleSetId)
            => await _db.SampleSets
                .Include(x => x.Collection)
                .Include(x => x.Collection.Organisation)
                .AnyAsync(x => x.Id == sampleSetId && x.Collection.Organisation.IsSuspended);

        // TODO: Figure out a better name
        /// <inheritdoc/>
        public async Task<bool> IsApiClient(int organisationId)
            => await _db.ApiClients.AnyAsync(x => x.Organisations.Any(y => y.OrganisationId == organisationId));

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListOpenRegistrationRequests()
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .Where(x =>
                    x.OrganisationTypeId == type.OrganisationTypeId &&
                    x.AcceptedDate == null &&
                    x.DeclinedDate == null && 
                    x.OrganisationCreatedDate == null)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequests()
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .AsNoTracking()
                .Where(x =>
                    x.OrganisationTypeId == type.OrganisationTypeId &&
                    x.AcceptedDate != null &&
                    x.DeclinedDate == null &&
                    x.OrganisationCreatedDate == null)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalRegistrationRequests()
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .Where(x =>
                    x.OrganisationTypeId == type.OrganisationTypeId &&
                    x.AcceptedDate != null &&
                    x.DeclinedDate != null)
                .ToListAsync();
        }
       
        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequest(int requestId)
            => await _db.OrganisationRegisterRequests.FirstOrDefaultAsync(x => x.OrganisationRegisterRequestId == requestId);

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequestByName(string name)
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .FirstOrDefaultAsync(x => x.OrganisationName == name && x.OrganisationTypeId == type.OrganisationTypeId);
        }

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequestByEmail(string email)
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .Where(x => x.OrganisationTypeId == type.OrganisationTypeId)
                .FirstOrDefaultAsync(x => x.UserEmail == email && x.DeclinedDate == null && x.OrganisationCreatedDate == null);
        }

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request)
        {
            _db.OrganisationRegisterRequests.Add(request);

            await _db.SaveChangesAsync();

            return request;
        }

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> UpdateRegistrationRequest(OrganisationRegisterRequest request)
        {
            var currentRequest = await _db.OrganisationRegisterRequests.FindAsync(request.OrganisationRegisterRequestId);

            currentRequest.UserName = request.UserName;
            currentRequest.UserEmail = request.UserEmail;
            currentRequest.OrganisationName = request.OrganisationName;
            currentRequest.OrganisationTypeId = request.OrganisationType?.OrganisationTypeId ?? request.OrganisationTypeId;
            
            currentRequest.RequestDate = request.RequestDate;
            currentRequest.AcceptedDate = request.AcceptedDate;
            currentRequest.OrganisationCreatedDate = request.OrganisationCreatedDate;
            currentRequest.DeclinedDate = request.DeclinedDate;

            await _db.SaveChangesAsync();

            return currentRequest;
        }

        /// <inheritdoc/>
        public async Task RemoveRegistrationRequest(OrganisationRegisterRequest request)
        {
            _db.OrganisationRegisterRequests.Attach(request);
            _db.OrganisationRegisterRequests.Remove(request);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> RegistrationRequestExists(string name)
        {
            var type = await GetOrganisationType();

            return await _db.OrganisationRegisterRequests
                .AnyAsync(x =>
                    x.OrganisationTypeId == type.OrganisationTypeId &&
                    x.OrganisationName == name &&
                    x.DeclinedDate == null);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegistrationReason>> ListRegistrationReasons(int organisationId)
            => await _db.OrganisationRegistrationReasons
                .Include(x => x.RegistrationReason)
                .Where(x => x.OrganisationId == organisationId)
                .ToListAsync();

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

        /// <inheritdoc/>
        public async Task<ApplicationUser> GetLastActiveUser(int organisationId)
        {
            var identityUsers = _userManager.Users.Where(x => x.LastLogin.HasValue);
            var organisationUsers = _db.OrganisationUsers.Where(x => x.OrganisationId == organisationId);

            return await identityUsers
                .Join(organisationUsers, iu => iu.Id, ou => ou.OrganisationUserId, (iu, ou) => iu)
                .OrderByDescending(x => x.LastLogin)
                .FirstOrDefaultAsync();
        }
    }
}
