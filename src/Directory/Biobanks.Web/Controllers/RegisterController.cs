using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Entities.Data;
using Directory.Identity.Contracts;
using Directory.Identity.Data.Entities;
using Directory.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.Register;
using Microsoft.AspNet.Identity;
using Biobanks.Web.Utilities;

namespace Biobanks.Web.Controllers
{
    public class RegisterController : ApplicationBaseController
    {
        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IEmailService _emailService;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        public RegisterController(
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService)
        {
            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _userManager = userManager;
            _emailService = emailService;
        }

        // GET: Register
        //public ActionResult Index() //We can use this later when ordinary people can register plain user accounts for themselves
        //{
        //    return View();
        //}

        //Register a biobank
        [AllowAnonymous]
        public ActionResult Biobank()
        {
            return View(new RegisterEntityModel
            {
                EntityName = "Biobank"
            });
        }


        //Register a biobank - ADAC Invite
        [UserAuthorize(Roles = "ADAC")]
        public ActionResult AdacBiobank()
        {
            return View("Biobank", new RegisterEntityModel
            {
                EntityName = "Biobank",
                AdacInvited = true
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [VerifyRecaptcha]
        public async Task<ActionResult> Biobank(RegisterEntityModel model)
        {
            model.EntityName = "Biobank";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check for duplicate Biobank name
            var existingOrg = await _biobankReadService.GetBiobankByNameAsync(model.Entity);

            if (existingOrg != null)
            {
                SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingOrg.ContactEmail} and ask them to add you as an admin.", FeedbackMessageType.Danger);

                return View(model);
            }

            //check for duplicate name against non-declined requests too
            if (await _biobankReadService.BiobankRegisterRequestExists(model.Entity))
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage(
                    $"Registration is already in progress for {model.Entity}. If you think this is in error please contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a>.",
                    FeedbackMessageType.Danger, true);

                return View(model);
            }

            //get the org type id for biobanks
            var type = await _biobankReadService.GetBiobankOrganisationTypeAsync();

            //Create a register request for this user and Biobank
            var request = await _biobankWriteService.AddRegisterRequestAsync(
                new OrganisationRegisterRequest
                {
                    UserName = model.Name,
                    UserEmail = model.Email,
                    OrganisationTypeId = type.OrganisationTypeId,
                    OrganisationName = model.Entity,
                    RequestDate = DateTime.Now
                });

            if (model.AdacInvited)
            {
                //ADAC Invited requests should be automatically accepted, and return to the ADAC Admin Requests view
                return RedirectToAction("AcceptBiobankRequest", "ADAC", new { requestId = request.OrganisationRegisterRequestId });
            }
            else
            {
                // Non ADAC Invited requests should notify ADAC users
                await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
                    model.Name,
                    model.Email,
                    model.Entity,
                    model.EntityName);
            }

            return View("RegisterConfirmation");
        }

        //Register a Network
        [AllowAnonymous]
        public ActionResult Network()
        {
            return View(new RegisterEntityModel
            {
                EntityName = "Network"
            });
        }

        //Register a biobank - ADAC Invite
        [UserAuthorize(Roles = "ADAC")]
        public ActionResult AdacNetwork()
        {
            return View("Network", new RegisterEntityModel
            {
                EntityName = "Network",
                AdacInvited = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [VerifyRecaptcha]
        public async Task<ActionResult> Network(RegisterEntityModel model)
        {
            ViewBag.RegisterEntityName = "network";

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //check for duplicate Network name
            var existingNetwork = await _biobankReadService.GetNetworkByNameAsync(model.Entity);

            if (existingNetwork != null)
            {
                SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingNetwork.Email} and ask them to add you as an admin.", FeedbackMessageType.Danger);
                return View(model);
            }

            //check for duplicate name against non-declined requests too
            if (await _biobankReadService.NetworkRegisterRequestExists(model.Entity))
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage($"Registration is already in progress for {model.Entity}. If you think this is in error please contact {supportEmail}.", FeedbackMessageType.Danger);

                return View(model);
            }

            //Create a register request for this user and Network
            var request = await _biobankWriteService.AddNetworkRegisterRequestAsync(
                new NetworkRegisterRequest
                {
                    UserName = model.Name,
                    UserEmail = model.Email,
                    NetworkName = model.Entity,
                    RequestDate = DateTime.Now
                });

            if (model.AdacInvited)
            {
                return RedirectToAction("AcceptNetworkRequest", "ADAC",
                    new { requestId = request.NetworkRegisterRequestId });
            }
            else
            {
                // Non ADAC Invited requests should notify ADAC users
                await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
                    model.Name,
                    model.Email,
                    model.Entity,
                    model.EntityName);
            }

            return View("RegisterConfirmation");
        }
    }
}
