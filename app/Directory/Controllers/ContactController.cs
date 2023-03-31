using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Contact;
using Biobanks.Directory.Models.Emails;
using Biobanks.Directory.Models.Home;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Biobanks.Directory.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly INetworkService _networkService;
        private readonly IOrganisationDirectoryService _organisationService;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly IEmailService _emailService; 
        
        public ContactController (
            INetworkService networkService,
            IOrganisationDirectoryService organisationService, 
            IMapper mapper,
            IMemoryCache memoryCache,
            IEmailService emailService
            )
        {
            _networkService = networkService;
            _organisationService = organisationService;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _emailService = emailService;
        }
        
        [HttpPost("EmailContactListAjax")]
        public async Task<ActionResult> EmailContactListAjax(OrganisationContactModel model)
        {
            // Convert IDs to list of Email Addresses
            var biobanks = await _organisationService.ListByExternalIds(model.Ids);
            var contacts = _mapper.Map<IEnumerable<ContactBiobankModel>>(biobanks);
            var contactlist = String.Join(", ", contacts.Select(c => c.ContactEmail));

            await _emailService.SendContactList(new EmailAddress(model.To), contactlist, model.ContactMe);

            return Ok(model);
        }
        
        [HttpGet("BiobankContactDetailsAjax/{id}")]
        public async Task<IActionResult> BiobankContactDetailsAjax(string id)
        {
            var biobankExternalIds = (List<string>)JsonConvert.DeserializeObject(id, typeof(List<string>));
            var biobanks = (await _organisationService.ListByExternalIds(biobankExternalIds)).ToList();

            var displayNetworks = _memoryCache.Get(ConfigKey.ContactThirdParty);
            var networkHandoverModels = new List<NetworkHandoverModel>();

            if (displayNetworks.ToString() == "true")
            {
                var networks = (await _networkService.List()).Where(n => n.ContactHandoverEnabled);

                foreach (var network in networks)
                {
                    var biobanksInNetwork = biobanks.Where(bb =>
                        network.OrganisationNetworks.Select(on => on.OrganisationId).Contains(bb.OrganisationId)).ToList();

                    var biobanksNotInNetwork = biobanks.Where(bb =>
                        !network.OrganisationNetworks.Select(on => on.OrganisationId).Contains(bb.OrganisationId)).ToList();

                    networkHandoverModels.Add(new NetworkHandoverModel
                    {
                        BiobankExternalIdsInNetwork = network.OrganisationNetworks
                            .Where(on => biobanksInNetwork.Select(b => b.OrganisationId).Contains(on.OrganisationId))
                            .Select(n => n.ExternalID)
                            .ToList(),
                        NonMemberBiobankAnonymousIds = biobanksNotInNetwork
                            .Where(bb => bb.AnonymousIdentifier.HasValue)
                            .Select(bb => (Guid)bb.AnonymousIdentifier)
                            .ToList(),

                        NetworkId = network.NetworkId,
                        NetworkName = network.Name,
                        LogoName = network.Logo,
                        HandoverBaseUrl = network.HandoverBaseUrl,
                        HandoverOrgIdsUrlParamName = network.HandoverOrgIdsUrlParamName,
                        MultipleHandoverOrdIdsParams = network.MultipleHandoverOrdIdsParams,
                        HandoverNonMembers = network.HandoverNonMembers,
                        HandoverNonMembersUrlParamName = network.HandoverNonMembersUrlParamName
                    });
                }
            }

            var model = new ContactModel
            {
                Contacts = _mapper.Map<IEnumerable<ContactBiobankModel>>(biobanks),
                Networks = networkHandoverModels.GroupBy(n => n.NetworkName).Select(n => n.First()).ToList()
            };

            return Ok(model);
        }

        [HttpPost("NotifyNetworkNonMembersOfHandoverAjax")]
        public async Task<ActionResult> NotifyNetworkNonMembersOfHandoverAjax(NetworkNonMemberContactModel model)
        {
            var network = await _networkService.Get(model.NetworkId);
            var biobanks = (await _organisationService.ListByAnonymousIdentifiers(model.BiobankAnonymousIdentifiers)).ToList();

            foreach (var biobank in biobanks) 
            {
                await _emailService.SendExternalNetworkNonMemberInformation(new EmailAddress(biobank.ContactEmail), biobank.Name,
                biobank.AnonymousIdentifier.ToString(), network.Name, network.Email, network.Description);
            }

            return NoContent();

        }
    }
}
