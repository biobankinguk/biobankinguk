﻿using Biobanks.Directory.Data;
using Biobanks.Directory.Services.Contracts;
using Biobanks.Entities.Data;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Contracts;
using Biobanks.Services.Dto;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web.WebSockets;

namespace Biobanks.Directory.Services
{
    public class NetworkService : INetworkService
    {
        private readonly IOrganisationService _organisationService;
        
        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;
        private readonly BiobanksDbContext _db;

        public NetworkService(
            IOrganisationService organisationService,
             IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            BiobanksDbContext db)
        {
            _organisationService = organisationService;
            _userManager = userManager;
            _db = db;
        }

        /// <inheritdoc/>
        public async Task<Network> Create(Network network)
        {
            _db.Networks.Add(network);
            await _db.SaveChangesAsync();

            return network;
        }

        /// <inheritdoc/>
        public async Task<Network> Update(Network network)
        {
            var exisitingNetwork = await Get(network.NetworkId);

            exisitingNetwork.LastUpdated = DateTime.Now;

            exisitingNetwork.Name = network.Name;
            exisitingNetwork.Email = network.Email;
            exisitingNetwork.Url = network.Url;
            exisitingNetwork.Logo = network.Logo;
            exisitingNetwork.Description = network.Description;
            exisitingNetwork.SopStatusId = network.SopStatus?.Id ?? network.SopStatusId;

            exisitingNetwork.ContactHandoverEnabled = network.ContactHandoverEnabled;
            exisitingNetwork.HandoverBaseUrl = network.HandoverBaseUrl;
            exisitingNetwork.HandoverOrgIdsUrlParamName = network.HandoverOrgIdsUrlParamName;
            exisitingNetwork.MultipleHandoverOrdIdsParams = network.MultipleHandoverOrdIdsParams;
            exisitingNetwork.HandoverNonMembers = network.HandoverNonMembers;
            exisitingNetwork.HandoverNonMembersUrlParamName = network.HandoverNonMembersUrlParamName;

            exisitingNetwork.OrganisationNetworks = network.OrganisationNetworks;

            await _db.SaveChangesAsync();
            // await _indexService.UpdateNetwork(network.NetworkId);

            return exisitingNetwork;
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
        public async Task<Network> GetByName(string networkName)
            => await _db.Networks
                .AsNoTracking()
                .Include(x => x.SopStatus)
                .FirstOrDefaultAsync(x => x.Name == networkName);

        /// <inheritdoc/>
        public async Task<IEnumerable<ApplicationUser>> ListAdmins(int networkId)
        {
            var adminIds = _db.NetworkUsers.Where(x => x.NetworkId == networkId).Select(x => x.NetworkUserId);

            return await _userManager.Users
                .AsNoTracking()
                .Join(adminIds, user => user.Id, adminId => adminId, (u, a) => u)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<NetworkRegisterRequest>> ListAcceptedRegistrationRequestsByUserId(string userId)
        {
            var userEmail = _userManager.Users.First(u => u.Id == userId).Email;
           
            return await _db.NetworkRegisterRequests
                .AsNoTracking()
                .Where(x => x.UserEmail == userEmail)
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
        public async Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(int organisationId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Network)
                .Where(x => x.OrganisationId == organisationId)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<IEnumerable<OrganisationNetwork>> ListOrganisationNetworks(IEnumerable<int> organisationIds)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .Include(x => x.Network)
                .Where(x => organisationIds.Contains(x.OrganisationId))
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<OrganisationNetwork> GetOrganisationNetwork(int organisationId, int networkId)
            => await _db.OrganisationNetworks
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.NetworkId == networkId);

        /// <inheritdoc/>
        public async Task<OrganisationNetwork> UpdateOrganisationNetwork(OrganisationNetwork organisationNetwork)
        {
            var existingON = await GetOrganisationNetwork(organisationNetwork.OrganisationId, organisationNetwork.NetworkId);

            existingON.ApprovedDate = organisationNetwork.ApprovedDate;
            existingON.ExternalID = organisationNetwork.ExternalID;

            await _db.SaveChangesAsync();

            return existingON;
        }

        /// <inheritdoc/>
        public async Task<bool> AddOrganisationToNetwork(int organisationId, int networkId, string biobankExternalID, bool approve)
        {
            if (await _organisationService.IsSuspended(organisationId))
                return false;

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
