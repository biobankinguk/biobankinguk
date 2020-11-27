using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Directory.Services.Contracts;
using Biobanks.Web.Models.Home;
using Biobanks.Web.Extensions;
using Newtonsoft.Json;
using Biobanks.Web.Utilities;
using Directory.Data.Constants;

namespace Biobanks.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public HomeController(IBiobankReadService biobankReadService,
            IMapper mapper,
            IEmailService emailService)
        {
            _biobankReadService = biobankReadService;
            _mapper = mapper;
            _emailService = emailService;
        }

        // GET: Home
        public async Task<ActionResult> Index()
        {
            return View(new HomepageContentModel
            {
                Title = Config.Get(ConfigKey.HomepageTitle, ""),
                SearchTitle = Config.Get(ConfigKey.HomepageSearchTitle, ""),
                ResourceRegistration = Config.Get(ConfigKey.HomepageResourceRegistration, ""),
                NetworkRegistration = Config.Get(ConfigKey.HomepageNetworkRegistration, ""),
            });
        }

        public async Task<ActionResult> About()
        {
            if (await _biobankReadService.GetSiteConfigStatus(ConfigKey.DisplayAboutPage) == true)
            {
                return View(new AboutModel
                {
                    BodyText = Config.Get(ConfigKey.AboutBodyText, "")
                });
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult Cookies() => View();

        public ActionResult Privacy() => View();

        public ViewResult Contact() => View();

        [HttpPost]
        public async Task<ActionResult> EmailContactListAjax(string email, List<string> ids, bool contactMe)
        {
            try
            {
                // Convert IDs to list of Email Addresses
                var biobanks = await _biobankReadService.GetBiobanksByExternalIdsAsync(ids);
                var contacts = _mapper.Map<IEnumerable<ContactBiobankModel>>(biobanks);
                var contactlist = String.Join(", ", contacts.Select(c => c.ContactEmail));

                await _emailService.SendContactList(email, contactlist, contactMe);
            }
            catch
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public async Task<JsonResult> BiobankContactDetailsAjax(string id)
        {
            var biobankExternalIds = (List<string>)JsonConvert.DeserializeObject(id, typeof(List<string>));
            var biobanks = (await _biobankReadService.GetBiobanksByExternalIdsAsync(biobankExternalIds)).ToList();
            
            var displayNetworks = Config.Get(ConfigKey.ContactThirdParty, "false") == "true";
            var networkHandoverModels = new List<NetworkHandoverModel>();

            if (displayNetworks)
            {
                var networks = (await _biobankReadService.ListNetworksAsync()).Where(n => n.ContactHandoverEnabled);

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

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> NotifyNetworkNonMembersOfHandoverAjax(NetworkNonMemberContactModel model)
        {
            var network = await _biobankReadService.GetNetworkByIdAsync(model.NetworkId);
            var biobanks = (await _biobankReadService.GetBiobanksByAnonymousIdentifiersAsync(model.BiobankAnonymousIdentifiers)).ToList();


            foreach (var biobank in biobanks)
            {
                await _emailService.SendExternalNetworkNonMemberInformation(biobank.ContactEmail, biobank.Name,
                    biobank.AnonymousIdentifier.ToString(), network.Name, network.Email, network.Description);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult FeedbackMessageAjax(string message, string type, bool html = false)
        {
            SetTemporaryFeedbackMessage(
                message,

                ((Func<FeedbackMessageType>)(() =>
               {
                   switch (type?.ToLower() ?? "")
                   {
                       case "success":
                           return FeedbackMessageType.Success;
                       case "danger":
                           return FeedbackMessageType.Danger;
                       case "warning":
                           return FeedbackMessageType.Warning;
                       default:
                           return FeedbackMessageType.Info;
                   }
               }))(),

                html);

            return PartialView("_FeedbackMessage");
        }
    }
}
