using AutoMapper;
using Biobanks.Data;
using Biobanks.Entities.Data;
using Biobanks.Entities.Shared;
using Biobanks.Search.Contracts;
using Biobanks.Search.Dto.PartialDocuments;
using Biobanks.Submissions.Api.Models.Directory;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Search.Dto.Documents;
using Biobanks.Submissions.Api.Services.Directory.Extensions;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Utilities.IdentityModel;

namespace Biobanks.Submissions.Api.Services.Directory
{
    public class OrganisationDirectoryService : IOrganisationDirectoryService
    {
        private readonly ICapabilityIndexProvider _capabilitiesIndex;
        private readonly ICollectionIndexProvider _collectionsIndex;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public OrganisationDirectoryService(
            ICapabilityIndexProvider capabilitiesIndex,
            ICollectionIndexProvider collectionsIndex,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _userManager = userManager;
            _capabilitiesIndex = capabilitiesIndex;
            _collectionsIndex = collectionsIndex;
            _db = db;
            _mapper = mapper;
        }

        private IQueryable<Organisation> Query()
            => _db.Organisations
                .Include(x => x.ApiClients)
                .Include(x => x.Funders)
                .Include(x => x.OrganisationAnnualStatistics)
                .Include(x => x.OrganisationType)
                .Include(x => x.County)
                .Include(x => x.Country)
                .Where(x => x.OrganisationType.Description == "Biobank");

        private IQueryable<Organisation> QueryForIndexing()
            => Query()
                .Include(x => x.Collections)
                .ThenInclude(c => c.SampleSets)
                .Include(x => x.DiagnosisCapabilities)
                .Include(x => x.OrganisationServiceOfferings)
                .ThenInclude(o => o.ServiceOffering);

        private IQueryable<OrganisationRegisterRequest> QueryRegistrationRequests()
            => _db.OrganisationRegisterRequests
                .Include(x => x.OrganisationType)
                .Where(x => x.OrganisationType.Description == "Biobank");

        private async Task<OrganisationType> GetOrganisationType()
            => await _db.OrganisationTypes.FirstOrDefaultAsync(x => x.Description == "Biobank");

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> List(string name = "", bool includeSuspended = true)
            => await Query()
                .AsNoTracking()
                .Where(x => x.Name.Contains(name) && (includeSuspended || x.IsSuspended == false))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListForActivity(string name = "", bool includeSuspended = true)
            => await Query()
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
        public async Task<IEnumerable<Organisation>> ListByUserId(string userId)
            => await _db.OrganisationUsers
                .AsNoTracking()
                .Include(x => x.Organisation)
                .Where(x => x.OrganisationUserId == userId)
                .Select(x => x.Organisation)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByExternalIds(IList<string> externalIds)
            => await Query()
                .AsNoTracking()
                .Include(x => x.OrganisationNetworks)
                .Where(x => externalIds.Contains(x.OrganisationExternalId))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Organisation>> ListByAnonymousIdentifiers(IEnumerable<Guid> identifiers)
            => await Query()
                .AsNoTracking()
                .Where(x => x.AnonymousIdentifier.HasValue && identifiers.Contains(x.AnonymousIdentifier.Value))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<Organisation> Get(int id)
            => await Query()
                .AsNoTracking()
                .Include(x => x.OrganisationRegistrationReasons)
                .Include(x => x.OrganisationServiceOfferings)
                .FirstOrDefaultAsync(x => x.OrganisationId == id);

        public async Task<Organisation> GetForBulkSubmissions(int id)
            => await Query()
            .AsNoTracking()
            .Include(x => x.AccessCondition)
            .Include(x => x.CollectionType)
            .Include(x => x.ApiClients)
            .FirstOrDefaultAsync(x => x.OrganisationId == id);

        /// <inheritdoc/>
        public async Task<Organisation> GetByName(string name)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == name);

        /// <inheritdoc/>
        public async Task<Organisation> GetByExternalId(string externalId)
            => await Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationExternalId == externalId);

        /// <inheritdoc/>
        public async Task<Organisation> Create(OrganisationDTO organisationDto)
        {
            var type = await GetOrganisationType();
            var organisation = _mapper.Map<Organisation>(organisationDto);

            organisation.OrganisationExternalId = "GBR-" + type.OrganisationTypeId + "-";
            organisation.OrganisationTypeId = type.OrganisationTypeId;
            organisation.AnonymousIdentifier = Guid.NewGuid();
            organisation.LastUpdated = DateTime.Now;

            _db.Organisations.Add(organisation);

            await _db.SaveChangesAsync();

            organisation.OrganisationExternalId += organisation.OrganisationId;

            return organisation;
        }

        /// <inheritdoc/>
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
                foreach (var orr in organisation.OrganisationRegistrationReasons ?? new List<OrganisationRegistrationReason>())
                {
                    var orgReason = allRegistrationReasons.SingleOrDefault(x => x.OrganisationId == orr.OrganisationId
                        && x.RegistrationReasonId == orr.RegistrationReasonId);
                    currentOrganisation.OrganisationRegistrationReasons.Add(orgReason);
                }

                var allServiceOfferings = await _db.OrganisationServiceOfferings.ToListAsync();
                foreach (var ser in organisation.OrganisationServiceOfferings ?? new List<OrganisationServiceOffering>())
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
                    if (organisation.Collections != null)
                        currentOrganisation.Collections
                            .SelectMany(c => c.SampleSets)
                            .ToList()
                            .ForEach(s => BackgroundJob.Enqueue(() => _collectionsIndex.Update(s.Id, partial)));

                    // Update Capabilities
                    if (organisation.DiagnosisCapabilities != null)
                        currentOrganisation.DiagnosisCapabilities
                            .ToList()
                            .ForEach(c => BackgroundJob.Enqueue(() => _capabilitiesIndex.Update(c.DiagnosisCapabilityId, partial)));
                }

            }
            return organisation;
        }

        /// <inheritdoc/>
        public async Task Delete(int id)
        {
            var organisation = await QueryForIndexing()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationId == id);

            if (organisation is null)
                throw new KeyNotFoundException($"No Organisation exists with Id={ organisation.OrganisationId }");

            // Remove From Search
            var batchSize = 100;

            // Remove SampleSets In Batches
            organisation.Collections
                .SelectMany(c => c.SampleSets.Select(s => s.Id))
                .Batch(batchSize)
                .ToList()
                .ForEach(batch => BackgroundJob.Enqueue(
                    () => _collectionsIndex.Delete(batch)));

            // Remove Capabilities In Batches
            organisation.DiagnosisCapabilities
                .Select(c => c.DiagnosisCapabilityId)
                .Batch(batchSize)
                .ToList()
                .ForEach(batch => BackgroundJob.Enqueue(
                    () => _capabilitiesIndex.Delete(batch))); ;

            // Remove From Database
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
        public async Task AddFunder(int funderId, int organisationId)
        {
            var organisation = await Query().FirstOrDefaultAsync(x => x.OrganisationId == organisationId);
            var funder = await _db.Funders.FirstOrDefaultAsync(x => x.Id == funderId);

            if (organisation is null)
                throw new KeyNotFoundException($"Organisation of Id={organisationId} does not exist");

            if (funder is null)
                throw new KeyNotFoundException($"Funder of Id={funderId} does not exist");

            // Add Organisation To Funder
            organisation.Funders.Add(funder);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task RemoveFunder(int funderId, int organisationId)
        {
            var organisation = await Query()
                .Include(x => x.Funders)
                .FirstOrDefaultAsync(x => x.OrganisationId == organisationId);

            if (organisation is null)
                throw new KeyNotFoundException($"Organisation of Id={organisationId} does not exist");

            // Remove Funder
            organisation.Funders.Remove(
                organisation.Funders.FirstOrDefault(x => x.Id == funderId));

            await _db.SaveChangesAsync();
        }

        private async Task<Organisation> Suspend(int organisationId, bool suspend)
        {
            var organisation = await Get(organisationId);
            organisation.IsSuspended = suspend;

            await Update(organisation);

            return organisation;
        }

        /// <inheritdoc/>
        public async Task<Organisation> Suspend(int organisationId)
            => await Suspend(organisationId, suspend: true);

        /// <inheritdoc/>
        public async Task<Organisation> Unsuspend(int organisationId)
            => await Suspend(organisationId, suspend: false);

        /// <inheritdoc/>
        public async Task<bool> IsSuspended(int organisationId)
            => await _db.Organisations.AnyAsync(x => x.OrganisationId == organisationId && x.IsSuspended);

        // TODO: Figure out a better name
        /// <inheritdoc/>
        public async Task<bool> IsApiClient(int organisationId)
            => await _db.ApiClients.AnyAsync(x => x.Organisations.Any(y => y.OrganisationId == organisationId));

        /// <inheritdoc/>
        public async Task<bool> UsesPublications(int organisationId)
            => await _db.Organisations.AnyAsync(x => x.OrganisationId == organisationId && !x.ExcludePublications);

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListOpenRegistrationRequests()
            => await QueryRegistrationRequests()
                .AsNoTracking()
                .Where(x =>
                    x.AcceptedDate == null &&
                    x.DeclinedDate == null &&
                    x.OrganisationCreatedDate == null)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequests()
            => await QueryRegistrationRequests()
                .AsNoTracking()
                .Where(x =>
                    x.AcceptedDate != null &&
                    x.DeclinedDate == null &&
                    x.OrganisationCreatedDate == null)
                .ToListAsync();
        
        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId)
        {
          var userEmail = _userManager.Users.First(u => u.Id == userId).Email;

          return await QueryRegistrationRequests()
            .AsNoTracking()
            .Where(x => x.UserEmail == userEmail)
            .Where(x => 
              x.AcceptedDate != null && 
              x.DeclinedDate == null && 
              x.OrganisationCreatedDate == null)
            .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegisterRequest>> ListHistoricalRegistrationRequests()
            => await QueryRegistrationRequests()
                .AsNoTracking()
                .Where(x =>
                    x.AcceptedDate != null &&
                    x.DeclinedDate != null)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequest(int requestId)
            => await QueryRegistrationRequests()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationRegisterRequestId == requestId);

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequestByName(string name)
            => await QueryRegistrationRequests()
                .FirstOrDefaultAsync(x => x.OrganisationName == name);

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> GetRegistrationRequestByEmail(string email)
            => await QueryRegistrationRequests()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserEmail == email && x.DeclinedDate == null && x.OrganisationCreatedDate == null);

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> AddRegistrationRequest(OrganisationRegisterRequest request)
        {
            // Set OrganisationType
            request.OrganisationType = await GetOrganisationType();

            // Add New Request
            _db.OrganisationRegisterRequests.Add(request);

            await _db.SaveChangesAsync();

            return request;
        }

        /// <inheritdoc/>
        public async Task<OrganisationRegisterRequest> UpdateRegistrationRequest(OrganisationRegisterRequest request)
        {
            var currentRequest = await QueryRegistrationRequests()
                .FirstOrDefaultAsync(x => x.OrganisationRegisterRequestId == request.OrganisationRegisterRequestId);

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
            => await QueryRegistrationRequests().AnyAsync(x => x.OrganisationName == name && x.DeclinedDate == null);

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationRegistrationReason>> ListRegistrationReasons(int organisationId)
            => await _db.OrganisationRegistrationReasons
                .AsNoTracking()
                .Include(x => x.RegistrationReason)
                .Where(x => x.OrganisationId == organisationId)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<KeyValuePair<string, string>> GenerateNewApiClient(int organisationId, string clientName = null)
        {
            var organisation = await Query().FirstOrDefaultAsync(x => x.OrganisationId == organisationId);

            if (organisation is null)
                throw new KeyNotFoundException();

            var clientId = Crypto.GenerateId();
            var clientSecret = Crypto.GenerateId();

            organisation.ApiClients.Add(new ApiClient
            {
                Name = clientName ?? clientId,
                ClientId = clientId,
                ClientSecretHash = clientSecret.Sha256(),

            });

            await _db.SaveChangesAsync();

            return new KeyValuePair<string, string>(clientId, clientSecret);
        }

        /// <inheritdoc/>
        public async Task<KeyValuePair<string, string>> GenerateNewSecretForBiobank(int organisationId)
        {
            var organisation = await Query().FirstOrDefaultAsync(x => x.OrganisationId == organisationId);

            if (organisation is null)
                throw new KeyNotFoundException();

            var credentials = organisation.ApiClients.First();
            var newSecret = Crypto.GenerateId();

            // Update Credentials Secret
            organisation.ApiClients.First().ClientSecretHash = newSecret.Sha256();

            await _db.SaveChangesAsync();

            return new KeyValuePair<string, string>(credentials.ClientId, newSecret);
        }

        /// <inheritdoc/>
        public async Task<ApplicationUser> GetLastActiveUser(int organisationId)
        {
            var userIds = await _db.OrganisationUsers
                .Where(x => x.OrganisationId == organisationId)
                .Select(x => x.OrganisationUserId)
                .ToListAsync();

            return await _userManager.Users
                .Where(x => userIds.Contains(x.Id) && x.LastLogin.HasValue)
                .OrderByDescending(x => x.LastLogin)
                .FirstOrDefaultAsync();
        }
    }
}
