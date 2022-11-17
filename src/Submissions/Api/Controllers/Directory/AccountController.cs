using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Constants;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.Directory
{
    public class AccountController : ApplicationBaseController
    {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserClaimsPrincipalFactory<ApplicationUser, IdentityRole> _claimsManager;

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
            UserClaimsPrincipalFactory<ApplicationUser, IdentityRole> claimsManager,
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
                var result = await _signinManager.PasswordSignInAsync(model.Email, model.Password, false, false);

                if (result.Succeeded)
                {
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
            var user = await _userManager.GetUserAsync(User);


            //Biobank

            //get all accepted biobanks
            var biobankRequests = await _organisationService.ListAcceptedRegistrationRequests();
            var firstAcceptedBiobabankRequest = biobankRequests.FirstOrDefault(x => x.UserName == user.Name && x.UserEmail == user.Email);

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
            var firstAcceptedNetworkRequest = networkRequests.FirstOrDefault(x => x.UserName == user.UserName && x.UserEmail == user.Email);

            // if there is an unregistered network to finish registering, go there
            if (firstAcceptedNetworkRequest != null)
            {
                Session[SessionKeys.ActiveOrganisationType] = ActiveOrganisationType.NewNetwork;
                Session[SessionKeys.ActiveOrganisationId] = firstAcceptedNetworkRequest.NetworkRegisterRequestId;
                Session[SessionKeys.ActiveOrganisationName] = firstAcceptedNetworkRequest.NetworkName;

                return RedirectToAction("SwitchToNetwork", new { id = firstAcceptedNetworkRequest.NetworkRegisterRequestId, newNetwork = true });
            }

            //ADAC
            if (HttpContext.User.IsInRole(Role.DirectoryAdmin))
                return RedirectToAction("Index", "ADAC");

            // if they have more than one claim, there's no obvious place to send them
            if (CurrentUser.Biobanks.Count() + CurrentUser.Networks.Count() != 1)
                return RedirectToAction("Index", "Home");

            //to returnUrl if appropriate, or a default route otherwise
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);

            // Biobank admin only
            if (HttpContext.User.IsInRole(Role.BiobankAdmin))
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
            if (HttpContext.User.IsInRole(Role.NetworkAdmin))
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
        public async Task<ActionResult> Logout(string returnUrl = null, bool isTimeout = false)
        {
            // Sign out of Identity
            await _signinManager.SignOutAsync();

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
            var user = await _userManager.FindByIdAsync(userId) ??
                throw new InvalidOperationException(
                $"Account could not be confirmed. User not found! User ID: {userId}");

            try
            {
                //check we have all the required params
                if (userId == null || token == null)
                    throw new ApplicationException();

                //Confirm the user
                var result = await _userManager.ConfirmEmailAsync(user, token);
                var tokenValidationId = await _tokenLog.EmailTokenValidated(token, userId);

                foreach (var error in result.Errors)
                {
                    var outError = error;
                    //Modify any errors if we want?
                    var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
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
                var passwordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

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
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

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

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

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
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password); //This also does historical password checks

                var tokenValidationId = await _tokenLog.PasswordTokenValidated(model.Token, user.Id);

                foreach (var error in result.Errors)
                {
                    //Format the error string from ResetPasswordAsync nicely
                    var htmlMessage = "<ul>";

                    Regex.Split(error, @"(?<=[.])").ToList().ForEach(e =>
                    {
                        if (!string.IsNullOrWhiteSpace(e))
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
            var signInStatus = await _signinManager.PasswordSignInAsync(user, model.Password, false, false);

            if (signInStatus.Succeeded)
            {
                return RedirectToAction("LoginRedirect", new { returnUrl });
            }
            else if (signInStatus.IsLockedOut)
            {
                SetTemporaryFeedbackMessage("This account has been locked out. Please wait and try again later.", FeedbackMessageType.Danger);
            }
            else if (signInStatus.IsNotAllowed)
            {
                var supportEmail = ConfigurationManager.AppSettings["AdacSupportEmail"];
                SetTemporaryFeedbackMessage(
                    "This account has not been confirmed. " +
                    $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = user.Email, returnUrl = Url.Action("Login") })}\">resend your confirmation link</a>, " +
                    $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                    "if you're having trouble.",
                    FeedbackMessageType.Warning,
                    true);
            }
            else
            {
                SetTemporaryFeedbackMessage("Email / password incorrect. Please try again.", FeedbackMessageType.Danger);
            }
            return View(model);

            
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
            var user = await _userManager.GetUserAsync(User);

            return new AccountDetailsModel
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        #endregion

    }
}