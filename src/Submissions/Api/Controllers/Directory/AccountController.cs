﻿using Biobanks.Submissions.Api.Auth.Entities;
using Biobanks.Submissions.Api.Controllers.Submissions;
using Biobanks.Submissions.Api.Identity;
using Biobanks.Submissions.Api.Models.Account;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.Directory
{
    public class AccountController : ApplicationBaseController
    {
        public ApplicationUserPrincipal CurrentUser => User.ToApplicationUserPrincipal();

        public IActionResult Index()
        {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomClaimsManager _claimsManager;

        private readonly INetworkService _networkService;
        private readonly IOrganisationDirectoryService _organisationService;

        private readonly IBiobankReadService _biobankReadService;
       // private readonly IEmailService _emailService;
        private readonly ITokenLoggingService _tokenLog;

        public AccountController(
            INetworkService networkService,
            IOrganisationDirectoryService organisationService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
        //    IEmailService emailService,
            CustomClaimsManager claimsManager,
            ITokenLoggingService tokenLog,
            IBiobankReadService biobankReadService)
        {
            _networkService = networkService;
            _organisationService = organisationService;

            _signinManager = signInManager;
            _userManager = userManager;
            _claimsManager = claimsManager;
       //     _emailService = emailService;
            _tokenLog = tokenLog;
            _biobankReadService = biobankReadService;
        }

        #region Login/Logout

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; //set this for any case in which we return a view

            if (ModelState.IsValid)
            {
                var result = await _signinManager.PasswordSignInAsync(model.Email, model.Password, false, true);
                var user = await _userManager.FindByNameAsync(model.Email) ?? 
                    throw new InvalidOperationException(
                      $"Successfully signed in user could not be retrieved! Username: {model.Email}");

                if (result.Succeeded)
                {
                    await _claimsManager.SetUserClaimsAsync(model.Email);
                    return RedirectToAction("LoginRedirect", new { returnUrl });
                }
                else if (result.IsLockedOut)
                {
                    SetTemporaryFeedbackMessage("This account has been locked out. Please wait and try again later.", FeedbackMessageType.Danger);
                }
                else if(result.IsNotAllowed)
                {
                    var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                    SetTemporaryFeedbackMessage(
                        "This account has not been confirmed. " +
                        $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = model.Email, returnUrl = Url.Action("Login") })}\">resend your confirmation link</a>, " +
                        $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                        "if you're having trouble.",
                        FeedbackMessageType.Warning,
                        true);
                }
                else
                {
                    SetTemporaryFeedbackMessage("Email / password incorrect. Please try again.", FeedbackMessageType.Danger);
                }
            }
            return View(model);
                   
              
        }

        public async Task<ActionResult> LoginRedirect(string returnUrl = null)
        {
            //This is an action, so that it's a separate request and the user identity cookie has roles and claims available :)
            //do we need them to create a profile for an associated org or network etc?

            // Start by updating user's last login time
            var user = await _userManager.GetUserAsync(HttpContext.User);
            
            await _userManager.UpdateLastLogin(CurrentUser.Identity.GetUserId());

            //Biobank

            //get all accepted biobanks
            var biobankRequests = await _organisationService.ListAcceptedRegistrationRequests();
            var firstAcceptedBiobabankRequest = biobankRequests.FirstOrDefault(x => x.UserName == CurrentUser.Name && x.UserEmail == CurrentUser.Email);

            // if there is an unregistered biobank to finish registering, go there
            if (firstAcceptedBiobabankRequest != null)
            {
                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.NewBiobank;
                Session[SessionKeys.ActiveOrganisationId] = firstAcceptedBiobabankRequest.OrganisationRegisterRequestId;
                Session[SessionKeys.ActiveOrganisationName] = firstAcceptedBiobabankRequest.OrganisationName;

                return RedirectToAction("SwitchToBiobank", new { id = firstAcceptedBiobabankRequest.OrganisationRegisterRequestId, newBiobank = true });
            }

            //get all accepted networks
            var networkRequests = await _networkService.ListAcceptedRegistrationRequests();
            var firstAcceptedNetworkRequest = networkRequests.FirstOrDefault(x => x.UserName == CurrentUser.Identity.GetUserName() && x.UserEmail == CurrentUser.Email);

            // if there is an unregistered network to finish registering, go there
            if (firstAcceptedNetworkRequest != null)
            {
                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.NewNetwork;
                Session[SessionKeys.ActiveOrganisationId] = firstAcceptedNetworkRequest.NetworkRegisterRequestId;
                Session[SessionKeys.ActiveOrganisationName] = firstAcceptedNetworkRequest.NetworkName;

                return RedirectToAction("SwitchToNetwork", new { id = firstAcceptedNetworkRequest.NetworkRegisterRequestId, newNetwork = true });
            }

            //ADAC
            if (CurrentUser.IsInRole(Role.ADAC.ToString()))
                return RedirectToAction("Index", "ADAC");

            // if they have more than one claim, there's no obvious place to send them
            if (CurrentUser.Biobanks.Count() + CurrentUser.Networks.Count() != 1)
                return RedirectToAction("Index", "Home");

            //to returnUrl if appropriate, or a default route otherwise
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            // Biobank admin only
            if (CurrentUser.IsInRole(Role.BiobankAdmin.ToString()))
            {
                var biobank = CurrentUser.Biobanks.FirstOrDefault();

                if (!biobank.Equals(default(KeyValuePair<int, string>)))
                {
                    Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Biobank;
                    Session[SessionKeys.ActiveOrganisationId] = biobank.Key;
                    Session[SessionKeys.ActiveOrganisationName] = biobank.Value;
                    return RedirectToAction("Collections", "Biobank");
                }
            }

            // Network admin only
            if (CurrentUser.IsInRole(Role.NetworkAdmin.ToString()))
            {
                var network = CurrentUser.Networks.FirstOrDefault();

                if (!network.Equals(default(KeyValuePair<int, string>)))
                {
                    Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Network;
                    Session[SessionKeys.ActiveOrganisationId] = network.Key;
                    Session[SessionKeys.ActiveOrganisationName] = network.Value;
                    return RedirectToAction("Biobanks", "Network");
                }
            }

            //or if no role-specific default
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous] //This may seem counter-intuitive but it fixes issues with session timeouts
        public ActionResult Logout(string returnUrl = null, bool isTimeout = false)
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();

            //send feedback if they were logged out due to session timeout
            if (isTimeout)
                SetTemporaryFeedbackMessage(
                    "Your session on the server expired due to inactivity, so you have been logged out.",
                    FeedbackMessageType.Info);
            else
                SetTemporaryFeedbackMessage(
                    "You have been logged out.",
                    FeedbackMessageType.Info);

            //send them on their way
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(returnUrl);
        }

        #endregion


        #region Account Confirmation

        [AllowAnonymous]
        public async Task<ActionResult> Confirm(string userId, string token)
        {
            try
            {
                //check we have all the required params
                if (userId == null || token == null)
                    throw new ApplicationException();

                //Confirm the user
                var result = await _userManager.ConfirmEmailAsync(userId, token);
                var tokenValidationId = await _tokenLog.EmailTokenValidated(token, userId);

                foreach (var error in result.Errors)
                {
                    var outError = error;
                    //Modify any errors if we want?
                    var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                    var user = await _userManager.FindByIdAsync(userId);
                    if (error == "Invalid token.")
                        outError =
                            $"Your account confirmation token is invalid or has expired. " +
                            $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = user.Email })}\">resend your confirmation link</a>, " +
                            $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                            "if you're having trouble.";

                    ModelState.AddModelError("", outError);
                    return View("GlobalErrors");
                }

                await _tokenLog.ValidationSuccessful(tokenValidationId);

                //pass them on to password reset, with a password token
                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(userId);

                await _tokenLog.PasswordTokenIssued(passwordToken, userId);

                return RedirectToAction("ResetPassword", new { userId, token = passwordToken });
            }
            catch (ApplicationException)
            {
                ModelState.AddModelError("", "There seems to be a problem with this confirmation link.");
                return View("GlobalErrors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("GlobalErrors");
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> ResendConfirmLink(string userEmail, bool onBehalf = false, string returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user.EmailConfirmed)
            {
                SetTemporaryFeedbackMessage("This account has already been confirmed.", FeedbackMessageType.Danger);
                if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            //Send new confirm email
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);

            await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

            await _emailService.ResendAccountConfirmation(
                user.Email,
                user.Name,
                Url.Action("Confirm", "Account",
                    new
                    {
                        userId = user.Id,
                        token = confirmToken
                    },
                    Request.Url.Scheme));

            SetTemporaryFeedbackMessage(
                onBehalf
                    ? $"{user.Name} ({user.Email}) has been sent a new confirmation link."
                    : "You have been sent a new confirmation link. Please check your email.",
                FeedbackMessageType.Success);

            if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region Password Management

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View(new ForgotPasswordModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !user.EmailConfirmed)
            {
                //Don't reveal no confirmed user with this email
                //should we resend confirmation email to unconfirmed?
                return View("ForgotPasswordConfirmation");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user.Id);

            await _tokenLog.PasswordTokenIssued(resetToken, user.Id);

            await _emailService.SendPasswordReset(
                model.Email,
                user.UserName,
                Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token = resetToken },
                    Request.Url.Scheme));

            return View("ForgotPasswordConfirmation");

        }

        [AllowAnonymous]
        public ActionResult ResetPassword(string userId, string token)
        {
            if (userId != null && token != null) return View(new ResetPasswordModel());

            ModelState.AddModelError("", "Invalid token or user id.");
            return View("GlobalErrors");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //get the user so we can do useful things with them!
            var user = await _userManager.FindByIdAsync(model.UserId);

            //Try and perform the reset
            try
            {
                var result = await _userManager.ResetPasswordAsync(user.Id, model.Token, model.Password); //This also does historical password checks

                var tokenValidationId = await _tokenLog.PasswordTokenValidated(model.Token, user.Id);

                foreach (var error in result.Errors)
                {
                    //Format the error string from ResetPasswordAsync nicely
                    var htmlMessage = "<ul>";

                    Regex.Split(error, @"(?<=[.])").ToList().ForEach(e =>
                    {
                        if (!e.IsNullOrWhiteSpace())
                            htmlMessage += $"<li>{e}</li>";
                    });

                    htmlMessage += "</ul>";

                    SetTemporaryFeedbackMessage(htmlMessage, FeedbackMessageType.Danger, true);
                    return View(model);
                }

                await _tokenLog.ValidationSuccessful(tokenValidationId);
            }
            catch (Exception e)
            {
                SetTemporaryFeedbackMessage(e.Message, FeedbackMessageType.Danger);
                return View(model);
            }

            //They've reset their password - we sign them in!
            var signInStatus = await _signinManager.LocalSignInAsync(user.Email, model.Password, false);

            switch (signInStatus)
            {
                case SignInStatus.Success:
                    await _claimsManager.SetUserClaimsAsync(user.Email);
                    return RedirectToAction("LoginRedirect");
                case SignInStatus.LockedOut:
                    SetTemporaryFeedbackMessage("This account has been locked out. Please wait and try again later.", FeedbackMessageType.Danger);
                    return View(model);
                case SignInStatus.RequiresVerification:
                    var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                    SetTemporaryFeedbackMessage(
                        "This account has not been confirmed. " +
                        $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = user.Email, returnUrl = Url.Action("Login") })}\">resend your confirmation link</a>, " +
                        $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                        "if you're having trouble.",
                        FeedbackMessageType.Warning,
                        true);
                    return View(model);
                default:
                    SetTemporaryFeedbackMessage("Email / password incorrect. Please try again.", FeedbackMessageType.Danger);
                    return View(model);
            }
        }

        #endregion


        #region Session Management

        public JsonResult KeepSessionAliveAjax()
        {
            return Json(new
            {
                success = true
            });
        }

        public double GetClientTimeoutAjax()
        {
            return 1200000; //20 mins in milliseconds
        }

        #endregion


        #region Access Control

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];

            SetTemporaryFeedbackMessage(
                "Access to the requested page was denied. If you think this is an error " +
                $"contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> ",
                FeedbackMessageType.Danger,
                true);

            return RedirectToAction("Index", "Home");
        }

        #endregion


        #region Account details

        public async Task<ActionResult> Index()
        {
            return View(await GetAccountDetailsModelAsync());
        }

        public async Task<ActionResult> Edit()
        {
            return View(await GetAccountDetailsModelAsync());
        }

        [HttpPost]
        public async Task<ActionResult> Edit(AccountDetailsModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Grab the user
            var user = await _userManager.FindByIdAsync(model.UserId);

            //update the object with the newly submitted values
            user.Name = model.Name;

            //update the user
            await _userManager.UpdateAsync(user);

            //Back to the profile to view your saved changes
            return RedirectToAction("Index");
        }

        private async Task<AccountDetailsModel> GetAccountDetailsModelAsync()
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Identity.GetUserId());

            return new AccountDetailsModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        #endregion


        #region Context switching

        public async Task<ActionResult> SwitchToBiobank(int id, bool newBiobank)
        {
            List<KeyValuePair<int, string>> biobanks;

            var userId = CurrentUser.Identity.GetUserId();
            var user = _userManager.FindById(userId);

            // Refresh user cookies - ensures user has correct Roles
            await _signinManager.RefreshSignInAsync(user);
            await _claimsManager.SetUserClaimsAsync(user.Email);

            // Get Biobank
            if (newBiobank)
            {
                var acceptedRequest = await _organisationService.ListAcceptedRegistrationRequests();

                biobanks = acceptedRequest
                    .Where(x => x.UserEmail == user.Email)
                    .Select(x => new KeyValuePair<int, string>(x.OrganisationRegisterRequestId, x.OrganisationName))
                    .ToList();
            }
            else
            {
                var organisations = await _organisationService.ListByUserId(userId);

                biobanks = organisations
                    .Select(x => new KeyValuePair<int, string>(x.OrganisationId, x.Name))
                    .ToList();
            }

            // if they don't have access to any biobanks, 403
            if (biobanks == null || biobanks.Count <= 0)
                return RedirectToAction("Forbidden");

            var biobank = biobanks.FirstOrDefault(o => o.Key == id);

            // if they don't have access to this biobank, 403
            if (biobank.Equals(default(KeyValuePair<int, string>)))
                return RedirectToAction("Forbidden");

            // else they do have access to this biobank - set session data and go to collections
            Session[SessionKeys.ActiveOrganisationId] = biobank.Key;
            Session[SessionKeys.ActiveOrganisationName] = biobank.Value;

            if (newBiobank)
                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.NewBiobank;
            else
                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.Biobank;

            return newBiobank ?
                RedirectToAction("Edit", "Biobank", new { detailsIncomplete = true })
                : RedirectToAction("Collections", "Biobank");
        }

        public async Task<ActionResult> SwitchToNetwork(int id, bool newNetwork)
        {
            var userId = CurrentUser.Identity.GetUserId();
            var user = _userManager.FindById(userId);

            // Refresh user cookies - ensures user has correct Roles
            await _signinManager.RefreshSignInAsync(user);
            await _claimsManager.SetUserClaimsAsync(user.Email);

            var networks = newNetwork
                ? (await _networkService.ListAcceptedRegistrationRequestsByUserId(userId))
                    .Select(x => (Id: x.NetworkRegisterRequestId, Name: x.NetworkName))
                : (await _networkService.ListByUserId(userId))
                    .Select(x => (Id: x.NetworkId, Name: x.Name));

            var network = networks.FirstOrDefault(x => x.Id == id);

            // if they don't have access to any biobanks, 403
            if (network == default)
                return RedirectToAction("Forbidden");

            // else they do have access to this biobank - set session data and go to collections
            Session[SessionKeys.ActiveOrganisationId] = network.Id;
            Session[SessionKeys.ActiveOrganisationName] = network.Name;

            Session[SessionKeys.ActiveOrganisationType] = newNetwork
                ? ActiveOrganisationType.NewNetwork
                : ActiveOrganisationType.Network;

            return newNetwork
                ? RedirectToAction("Edit", "Network", new { detailsIncomplete = true })
                : RedirectToAction("Biobanks", "Network");
        }

        #endregion

    }
}