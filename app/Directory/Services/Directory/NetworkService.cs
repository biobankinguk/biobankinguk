using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Data;
using Biobanks.Data.Entities;
using Biobanks.Directory.Search.Dto.Documents;
using Biobanks.Directory.Search.Dto.PartialDocuments;
using Biobanks.Directory.Search.Legacy;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.Directory.Dto;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biobanks.Directory.Services.Directory
{
    public class NetworkService : INetworkService
    {
        private readonly IIndexProvider _indexProvider;

        private readonly UserManager<ApplicationUser>  _userManager;
        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        public NetworkService(
            IIndexProvider indexProvider,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db,
            IMapper mapper)
        {
            _indexProvider = indexProvider;
            _userManager = userManager;
            _db = db;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Network> Create(NetworkDTO networkDto)
        {
            var network = _mapper.Map<Network>(networkDto);

            _db.Networks.Add(network);

            await _db.SaveChangesAsync();

            return network;
        }

        /// <inheritdoc/>
        public async Task<Network> Update(NetworkDTO networkDto)
        {
            var network = await _db.Networks
                .Include(x => x.OrganisationNetworks)
                .FirstOrDefaultAsync(x => x.NetworkId == networkDto.NetworkId);

            if (network is null)
                throw new KeyNotFoundException($"No Network with Id={networkDto.NetworkId}");

            // Map Updates To Existing Organisation
            _mapper.Map(networkDto, network);

            // Set Timestamp
            network.LastUpdated = DateTime.Now;

            await _db.SaveChangesAsync();

            // Update Indexing Of Network
            network = await GetForIndexing(network.NetworkId);

            foreach (var organisation in network.OrganisationNetworks.Select(x => x.Organisation))
            {
                var partialNetworks = new PartialNetworks
                {
                    Networks = organisation.OrganisationNetworks.Select(x => new NetworkDocument
                    {
                        Name = x.Network.Name
                    })
                };

                // Index Collections
                organisation.Collections
                    .SelectMany(c => c.SampleSets)
                    .ToList()
                    .ForEach(s => BackgroundJob.Enqueue(() =>
                        _indexProvider.UpdateCollectionSearchDocument(s.Id, partialNetworks)));

                // Index Capabilities
                organisation.DiagnosisCapabilities
                    .ToList()
                    .ForEach(c => BackgroundJob.Enqueue(() =>
                        _indexProvider.UpdateCollectionSearchDocument(c.DiagnosisCapabilityId, partialNetworks)));

            }

            return network;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Network>> List()
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.OrganisationNetworks)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Network>> ListByUserId(string userId)
            => await _db.NetworkUsers
                .Include(x => x.Network)
                .Where(x => x.NetworkUserId == userId)
                .Select(x => x.Network)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<Network>> ListByOrganisationId(int organisationId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Network)
                .Where(x => x.OrganisationId == organisationId)
                .Select(x => x.Network)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<Network> Get(int networkId)
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.SopStatus)
                .FirstOrDefaultAsync(x => x.NetworkId == networkId);

        /// <inheritdoc/>
        public async Task<Network> GetForIndexing(int networkId)
            => await _db.Networks
                .Include(x => x.SopStatus)
                .Include(x => x.OrganisationNetworks)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation.Collections)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation.Collections)
                        .ThenInclude(c => c.SampleSets)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation.DiagnosisCapabilities)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation.OrganisationServiceOfferings)
                .Include(x => x.OrganisationNetworks)
                    .ThenInclude(y => y.Organisation.OrganisationServiceOfferings)
                        .ThenInclude(o => o.ServiceOffering)
                .FirstOrDefaultAsync(x => x.NetworkId == networkId);

        /// <inheritdoc/>
        public async Task<Network> GetByName(string networkName)
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.SopStatus)
                .FirstOrDefaultAsync(x => x.Name == networkName);

        /// <inheritdoc/>
        public async Task<IEnumerable<ApplicationUser>> ListAdmins(int networkId)
        {
            var adminIds = await _db.NetworkUsers
                .Where(x => x.NetworkId == networkId)
                .Select(x => x.NetworkUserId)
                .ToListAsync();

            return await _userManager.Users
                .AsNoTracking()
                .Where(x => adminIds.Contains(x.Id))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId)
        {
            var userEmail = _userManager.Users.First(u => u.Id == userId).Email;

            return await _db.NetworkRegisterRequests
                .AsNoTracking()
                .Where(x => EF.Functions.ILike(x.UserEmail, userEmail))
                .Where(x => x.AcceptedDate != null && x.DeclinedDate == null && x.NetworkCreatedDate == null)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequests()
            => await _db.NetworkRegisterRequests
                .AsNoTracking()
                .Where(x => x.AcceptedDate != null && x.DeclinedDate == null && x.NetworkCreatedDate == null)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<NetworkRegisterRequest>> ListHistoricalRegistrationRequests()
             => await _db.NetworkRegisterRequests
                .AsNoTracking()
                .Where(x => x.AcceptedDate != null || x.DeclinedDate != null)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<NetworkRegisterRequest>> ListOpenRegistrationRequests()
             => await _db.NetworkRegisterRequests
                .AsNoTracking()
                .Where(x => x.AcceptedDate == null && x.DeclinedDate == null && x.NetworkCreatedDate == null)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<NetworkRegisterRequest> GetRegistrationRequest(int requestId)
            => await _db.NetworkRegisterRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.NetworkRegisterRequestId == requestId);

        /// <inheritdoc/>
        public async Task<NetworkRegisterRequest> GetRegistrationRequestByEmail(string email)
            => await _db.NetworkRegisterRequests
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserEmail == email && x.DeclinedDate == null && x.NetworkCreatedDate == null);

        /// <inheritdoc/>
        public async Task<NetworkRegisterRequest> AddRegistrationRequest(NetworkRegisterRequest request)
        {
            _db.NetworkRegisterRequests.Add(request);
            await _db.SaveChangesAsync();

            return request;
        }

        /// <inheritdoc/>
        public async Task<NetworkRegisterRequest> UpdateRegistrationRequest(NetworkRegisterRequest request)
        {
            var currentRequest = await _db.NetworkRegisterRequests
                .FirstOrDefaultAsync(x => x.NetworkRegisterRequestId == request.NetworkRegisterRequestId);

            _mapper.Map(request, currentRequest);

            await _db.SaveChangesAsync();

            return currentRequest;
        }

        /// <inheritdoc/>
        public async Task<bool> HasActiveRegistrationRequest(string name)
            => await _db.NetworkRegisterRequests.AnyAsync(x => x.NetworkName == name && x.DeclinedDate == null);

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(int organisationId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Network)
                .Where(x => x.OrganisationId == organisationId)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<OrganisationNetwork> GetOrganisationNetwork(int organisationId, int networkId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.NetworkId == networkId);

        /// <inheritdoc/>
        public async Task<OrganisationNetwork> UpdateOrganisationNetwork(OrganisationNetwork organisationNetwork)
        {
            var existingON = await _db.OrganisationNetworks
                .Where(x => x.OrganisationId == organisationNetwork.OrganisationId && x.NetworkId == organisationNetwork.NetworkId)
                .FirstOrDefaultAsync();

            existingON.ApprovedDate = organisationNetwork.ApprovedDate;
            existingON.ExternalID = organisationNetwork.ExternalID;

            await _db.SaveChangesAsync();

            return existingON;
        }

        /// <inheritdoc/>
        public async Task<bool> AddOrganisationToNetwork(int organisationId, int networkId, string biobankExternalID, bool approve)
        {

            var organisationNetwork = new OrganisationNetwork
            {
                NetworkId = networkId,
                OrganisationId = organisationId,
                ExternalID = biobankExternalID,
            };

            if (approve)
                organisationNetwork.ApprovedDate = DateTime.Now;

            _db.OrganisationNetworks.Add(organisationNetwork);

            await _db.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task RemoveOrganisationFromNetwork(int organisationId, int networkId)
        {
            // TODO: Better End Condition?
            if (!await _db.Organisations.AnyAsync(x => x.OrganisationId == organisationId && !x.IsSuspended))
                throw new ApplicationException();

            // Remove The Join Relationship For Given Organisation / Network
            _db.OrganisationNetworks.RemoveRange(
                _db.OrganisationNetworks.Where(x => x.OrganisationId == organisationId && x.NetworkId == networkId));

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<NetworkUser> AddNetworkUser(string userId, int networkId)
        {
            // TODO: Validate the id's? user is more annoying as needs userManager
            var nu = new NetworkUser
            {
                NetworkId = networkId,
                NetworkUserId = userId
            };

            _db.NetworkUsers.Add(nu);
            await _db.SaveChangesAsync();

            return nu;
        }

        /// <inheritdoc/>
        public async Task RemoveNetworkUser(string userId, int networkId)
        {
            var users = await _db.NetworkUsers
                .Where(x => x.NetworkUserId == userId && x.NetworkId == networkId)
                .ToListAsync();

            _db.NetworkUsers.RemoveRange(users);

            await _db.SaveChangesAsync();
        }

    }
}
