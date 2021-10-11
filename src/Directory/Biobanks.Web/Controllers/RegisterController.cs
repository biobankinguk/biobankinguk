using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using Biobanks.Entities.Data;
using Biobanks.Identity.Contracts;
using Biobanks.Identity.Data.Entities;
using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.Register;
using Microsoft.AspNet.Identity;
using Biobanks.Web.Utilities;
using Biobanks.Directory.Data.Constants;
using Biobanks.Directory.Services.Contracts;

namespace Biobanks.Web.Controllers
{
    public class RegisterController : ApplicationBaseController
    {
        private readonly INetworkService _networkService;
        private readonly IOrganisationService _organisationService;

        private readonly IBiobankReadService _biobankReadService;
        private readonly IBiobankWriteService _biobankWriteService;
        private readonly IEmailService _emailService;
        private readonly IRegistrationDomainService _registrationDomainService;
        private readonly IConfigService _configService;

        private readonly IApplicationUserManager<ApplicationUser, string, IdentityResult> _userManager;

        public RegisterController(
            INetworkService networkService,
            IOrganisationService organisationService,
            IBiobankReadService biobankReadService,
            IBiobankWriteService biobankWriteService,
            IApplicationUserManager<ApplicationUser, string, IdentityResult> userManager,
            IEmailService emailService, 
            IRegistrationDomainService registrationDomainService,
            IConfigService configService)
        {
            _networkService = networkService;
            _organisationService = organisationService;

            _biobankReadService = biobankReadService;
            _biobankWriteService = biobankWriteService;
            _userManager = userManager;
            _emailService = emailService;
            _registrationDomainService = registrationDomainService;
            _configService = configService;
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

        public async Task<bool> RegistrationHoneypotTrap(RegisterEntityModel model)
        {
            //check if honeypot field is true
            if (model.AcceptTerms && !model.AdacInvited)
            {
                //check if domain rule exist for user
                var rule = await _registrationDomainService.GetRuleByValue(model.Email);

                if (rule != null)
                {
                    //update rule to block user
                    rule.RuleType = "Block";
                    rule.DateModified = DateTime.Now;
                    rule.Source = "Automatic block: honeypot";
                    await _registrationDomainService.Update(rule);
                }
                else
                {
                    // add new rule to block user
                    await _registrationDomainService.Add(new RegistrationDomainRule
                    {
                        RuleType = "Block",
                        Source = "Automatic block: honeypot",
                        Value = model.Email,
                        DateModified = DateTime.Now
                    });
                }

                return true;
            }

            return false;
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

            //check for honeypot trap
            if (await RegistrationHoneypotTrap(model))
                return View("RegisterConfirmation");

            //check for duplicate Biobank name
            var existingOrg = await _organisationService.GetByName(model.Entity);

            if (existingOrg != null)
            {
                SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingOrg.ContactEmail} and ask them to add you as an admin.", FeedbackMessageType.Danger);

                return View(model);
            }

            if (Uri.IsWellFormedUriString(model.Name, UriKind.Absolute) || Uri.IsWellFormedUriString(model.Entity, UriKind.Absolute))
            {
                SetTemporaryFeedbackMessage("The admin name or organisation name fields cannot be URIs. Please enter a non URI value.", FeedbackMessageType.Danger);
                return View(model);
            }

            //check for duplicate name against non-declined requests too
            if (await _organisationService.RegistrationRequestExists(model.Entity))
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage(
                    $"Registration is already in progress for {model.Entity}. If you think this is in error please contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a>.",
                    FeedbackMessageType.Danger, true);

                return View(model);
            }

            //Check if email is on the allow/block list
            if (!await _registrationDomainService.ValidateEmail(model.Email) && !model.AdacInvited)
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage(
                    $"Sorry, registrations from this email domain are not allowed. If you think this is in error please contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a>.",
                    FeedbackMessageType.Danger, true);

                return View(model);
            }

            //Create a register request for this user and Biobank
            var request = await _organisationService.AddRegistrationRequest(
                new OrganisationRegisterRequest
                {
                    UserName = model.Name,
                    UserEmail = model.Email,
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
                if (await _configService.GetFlagConfigValue(ConfigKey.RegistrationEmails) == true)
                {
                    // Non ADAC Invited requests should notify ADAC users
                    await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
                        model.Name,
                        model.Email,
                        model.Entity,
                        model.EntityName);
                }
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

            //check for honeypot trap
            if (await RegistrationHoneypotTrap(model))
                return View("RegisterConfirmation");

            //check for duplicate Network name
            var existingNetwork = await _networkService.GetByName(model.Entity);

            if (existingNetwork != null)
            {
                SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingNetwork.Email} and ask them to add you as an admin.", FeedbackMessageType.Danger);
                return View(model);
            }

            //check for duplicate name against non-declined requests too
            if (await _networkService.HasActiveRegistrationRequest(model.Entity))
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage($"Registration is already in progress for {model.Entity}. If you think this is in error please contact {supportEmail}.", FeedbackMessageType.Danger);

                return View(model);
            }

            //Check if email is on the allow/block list
            if (!await _registrationDomainService.ValidateEmail(model.Email) && !model.AdacInvited)
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage(
                    $"Sorry, registrations from this email domain are not allowed. If you think this is in error please contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a>.",
                    FeedbackMessageType.Danger, true);

                return View(model);
            }

            //Create a register request for this user and Network
            var request = await _networkService.AddRegistrationRequest(
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
                if (await _configService.GetFlagConfigValue(ConfigKey.RegistrationEmails) == true)
                {
                    // Non ADAC Invited requests should notify ADAC users
                    await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
                    model.Name,
                    model.Email,
                    model.Entity,
                    model.EntityName);
                }
            }

            return View("RegisterConfirmation");
        }
    }
}
