using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Directory.Services
{
    public class NetworkService : INetworkService
    {
        private readonly IOrganisationService _organisationService;

        private readonly BiobanksDbContext _db;

        public NetworkService(
            IOrganisationService organisationService,
            BiobanksDbContext db)
        {
            _organisationService = organisationService;
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Network>> List()
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.OrganisationNetworks)
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
        public async Task<Network> GetByName(string networkName)
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.SopStatus)
                .FirstOrDefaultAsync(x => x.Name == networkName);

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

            // TODO: Figure Out Mapping

            await _db.SaveChangesAsync();

            return currentRequest;
        }

        /// <inheritdoc/>
        public async Task DeleteRegistrationRequest(NetworkRegisterRequest request)
        {
            _db.NetworkRegisterRequests.Attach(request);
            _db.NetworkRegisterRequests.Remove(request);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> HasActiveRegistrationRequest(string name)
            => await _db.NetworkRegisterRequests.AnyAsync(x => x.NetworkName == name && x.DeclinedDate == null);

        /// <inheritdoc/>
        public async Task<bool> AddOrganisationToNetwork(int biobankId, int networkId, string biobankExternalID, bool approve)
        {
            if (await _organisationService.IsSuspended(biobankId))
                return false;

            var organisationNetwork = new OrganisationNetwork
            {
                NetworkId = networkId,
                OrganisationId = biobankId,
                ExternalID = biobankExternalID,
            };

            if (approve)
                organisationNetwork.ApprovedDate = DateTime.Now;

            _db.OrganisationNetworks.Add(organisationNetwork);

            await _db.SaveChangesAsync();

            return true;
        }

        /// <inheritdoc/>
        public async Task<NetworkUser> AddUserToNetwork(string userId, int networkId)
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









        public Task<Network> CreateNetworkAsync(Network network)
        {
            throw new System.NotImplementedException();
        }



        public List<KeyValuePair<int, string>> GetAcceptedNetworkRequestIdsAndNamesByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Organisation>> GetBiobanksByNetworkIdForIndexingAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }

       

        public List<KeyValuePair<int, string>> GetNetworkIdsAndNamesByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }





        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworkAsync(int biobankId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(IEnumerable<int> organisationIds)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<OrganisationNetwork>> GetOrganisationNetworksAsync(int biobankId)
        {
            throw new System.NotImplementedException();
        }

       
        public Task<IEnumerable<ApplicationUser>> ListNetworkAdminsAsync(int networkId)
        {
            throw new System.NotImplementedException();
        }







        public Task RemoveBiobankFromNetworkAsync(int biobankId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveUserFromNetworkAsync(string userId, int networkId)
        {
            throw new System.NotImplementedException();
        }

        public Task<Network> UpdateNetworkAsync(NetworkDTO networkDto)
        {
            throw new System.NotImplementedException();
        }

        public Task<OrganisationNetwork> UpdateOrganisationNetworkAsync(OrganisationNetwork organisationNetwork)
        {
            throw new System.NotImplementedException();
        }
    }
}
