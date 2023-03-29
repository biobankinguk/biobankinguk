using System;
using System.Threading.Tasks;
using Biobanks.Data.Entities;
using Biobanks.Directory.Auth;
using Biobanks.Directory.Config;
using Biobanks.Directory.Models.Register;
using Biobanks.Directory.Services.Directory.Contracts;
using Biobanks.Directory.Services.EmailServices.Contracts;
using Biobanks.Directory.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Biobanks.Directory.Controllers.Directory;

public class RegisterController : Controller
{
  private readonly INetworkService _networkService;
  private readonly IOrganisationDirectoryService _organisationService;
  private readonly IEmailService _emailService;
  private readonly IRegistrationDomainService _registrationDomainService;
  private readonly IConfigService _configService;
  private readonly SitePropertiesOptions _siteConfig;
  private readonly IRecaptchaService _recaptchaService;

  public RegisterController(
    INetworkService networkService,
    IOrganisationDirectoryService organisationService,
    IEmailService emailService, 
    IRegistrationDomainService registrationDomainService,
    IConfigService configService,
    IOptions<SitePropertiesOptions> siteConfigOptions,
    IRecaptchaService recaptchaService
    )
  {
    _networkService = networkService;
    _organisationService = organisationService;
    _emailService = emailService;
    _registrationDomainService = registrationDomainService;
    _configService = configService;
    _siteConfig = siteConfigOptions.Value;
    _recaptchaService = recaptchaService;
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
  [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
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
  public async Task<ActionResult> Biobank(RegisterEntityModel model)
  {
      model.EntityName = "Biobank";

      //Verify for Google Recaptcha
      var recaptchaToken = HttpContext.Request.Form["g-recaptcha-response"];
      var isRecaptchaValid = await _recaptchaService.VerifyToken(recaptchaToken);

      if (!isRecaptchaValid.Success)
      {
          foreach (var error in isRecaptchaValid.Errors)
          {
              ModelState.AddModelError("ReCAPTCHA", error);
          }
          
          return View(model);
      }

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
          this.SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingOrg.ContactEmail} and ask them to add you as an admin.", FeedbackMessageType.Danger);

          return View(model);
      }

      if (Uri.IsWellFormedUriString(model.Name, UriKind.Absolute) || Uri.IsWellFormedUriString(model.Entity, UriKind.Absolute))
      {
          this.SetTemporaryFeedbackMessage("The admin name or organisation name fields cannot be URIs. Please enter a non URI value.", FeedbackMessageType.Danger);
          return View(model);
      }

      //check for duplicate name against non-declined requests too
      if (await _organisationService.RegistrationRequestExists(model.Entity))
      {
          var supportEmail = _siteConfig.SupportAddress;
          this.SetTemporaryFeedbackMessage(
              $"Registration is already in progress for {model.Entity}. If you think this is in error please contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a>.",
              FeedbackMessageType.Danger, true);

          return View(model);
      }

      //Check if email is on the allow/block list
      if (!await _registrationDomainService.ValidateEmail(model.Email) && !model.AdacInvited)
      {
          var supportEmail = _siteConfig.SupportAddress;
          this.SetTemporaryFeedbackMessage(
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
          return RedirectToAction("AcceptBiobankRequest", "Requests", new { Area = "Admin", requestId = request.OrganisationRegisterRequestId });
      }
      else
      {
          if (await _configService.GetFlagConfigValue(ConfigKey.RegistrationEmails) == true)
          {
              // Non ADAC Invited requests should notify ADAC users
              await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
                  model.Name,
                  model.Email,
                  model.EntityName,
                  model.Entity);
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
  [Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
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
  public async Task<ActionResult> Network(RegisterEntityModel model)
  {
      ViewBag.RegisterEntityName = "network";
      
      //Verify for Google Recaptcha
      var recaptchaToken = HttpContext.Request.Form["g-recaptcha-response"];
      var isRecaptchaValid = await _recaptchaService.VerifyToken(recaptchaToken);

      if (!isRecaptchaValid.Success)
      {
          foreach (var error in isRecaptchaValid.Errors)
          {
              ModelState.AddModelError("ReCAPTCHA", error);
          }
          
          return View(model);
      }

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
          this.SetTemporaryFeedbackMessage($"{model.Entity} already exists. Please contact {existingNetwork.Email} and ask them to add you as an admin.", FeedbackMessageType.Danger);
          return View(model);
      }

      //check for duplicate name against non-declined requests too
      if (await _networkService.HasActiveRegistrationRequest(model.Entity))
      {
          var supportEmail = _siteConfig.SupportAddress;
          this.SetTemporaryFeedbackMessage($"Registration is already in progress for {model.Entity}. If you think this is in error please contact {supportEmail}.", FeedbackMessageType.Danger);
          
          return View(model);
      }

      //Check if email is on the allow/block list
      if (!await _registrationDomainService.ValidateEmail(model.Email) && !model.AdacInvited)
      {
          var supportEmail = _siteConfig.SupportAddress;
          this.SetTemporaryFeedbackMessage(
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
          return RedirectToAction("AcceptNetworkRequest", "Requests",
              new { Area= "Admin", requestId = request.NetworkRegisterRequestId });
      }
      else
      {
          if (await _configService.GetFlagConfigValue(ConfigKey.RegistrationEmails) == true)
          {
              // Non Admin Invited requests should notify Admin users
              await _emailService.SendDirectoryAdminNewRegisterRequestNotification(
              model.Name,
              model.Email,
              model.EntityName,
              model.Entity);
          }
      }

      return View("RegisterConfirmation");
  }

}
