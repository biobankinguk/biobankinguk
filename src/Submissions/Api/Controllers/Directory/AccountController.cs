using Biobanks.Data.Entities;
using Biobanks.Submissions.Api.Config;
using Biobanks.Submissions.Api.Models.Account;
using Biobanks.Submissions.Api.Models.Emails;
using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Services.EmailServices.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Biobanks.Submissions.Api.Controllers.Directory
{
  public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signinManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly ITokenLoggingService _tokenLog;
        private readonly SitePropertiesOptions _siteConfig;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            ITokenLoggingService tokenLog,
            IOptions<SitePropertiesOptions> siteConfigOptions
        )
        {
            _signinManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _tokenLog = tokenLog;
            _siteConfig = siteConfigOptions.Value;
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
                var user = await _userManager.FindByNameAsync(model.Email);

                if (result.Succeeded)
                {
                  if (user is null)
                    throw new InvalidOperationException(
                    $"Successfully signed in user could not be retrieved! User Email: {model.Email}");

                  // Update users last login
                  user.LastLogin = DateTime.UtcNow;
                  await _userManager.UpdateAsync(user);

                  return RedirectToAction("Index", "Home");
                }

                else if (result.IsLockedOut)
                {
                    this.SetTemporaryFeedbackMessage("This account has been locked out. Please wait and try again later.", FeedbackMessageType.Danger);
                }
                else if(result.IsNotAllowed)
                {
                    var supportEmail = _siteConfig.SupportAddress;
                    this.SetTemporaryFeedbackMessage(
                        "This account has not been confirmed. " +
                        $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = model.Email, returnUrl = Url.Action("Login") })}\">resend your confirmation link</a>, " +
                        $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                        "if you're having trouble.",
                        FeedbackMessageType.Warning,
                        true);
                }
                else
                {
                    this.SetTemporaryFeedbackMessage("Email / password incorrect. Please try again.", FeedbackMessageType.Danger);
                }
            }
            return View(model);
                   
              
        }


        [AllowAnonymous] //This may seem counter-intuitive but it fixes issues with session timeouts
        public async Task<ActionResult> Logout(string returnUrl = null, bool isTimeout = false)
        {
            // Sign out of Identity
            await _signinManager.SignOutAsync();

            //send feedback if they were logged out due to session timeout
            if (isTimeout)
                this.SetTemporaryFeedbackMessage(
                    "Your session on the server expired due to inactivity, so you have been logged out.",
                    FeedbackMessageType.Info);
            else
                this.SetTemporaryFeedbackMessage(
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
                    var supportEmail = _siteConfig.SupportAddress;
                    if (error.Description == "Invalid token.")
                        outError.Description =
                            $"Your account confirmation token is invalid or has expired. " +
                            $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = user.Email })}\">resend your confirmation link</a>, " +
                            $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                            "if you're having trouble.";

                    ModelState.AddModelError("", outError.Description);
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
                this.SetTemporaryFeedbackMessage("This account has already been confirmed.", FeedbackMessageType.Danger);
                if (returnUrl != null && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index", "Home");
            }

            //Send new confirm email
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            await _tokenLog.EmailTokenIssued(confirmToken, user.Id);

            await _emailService.ResendAccountConfirmation(
                new EmailAddress(user.Email),
                user.Name,
                Url.Action("Confirm", "Account", 
                  new 
                  {
                      userId = user.Id,
                      token = confirmToken
                  },
                  Request.Scheme));

            this.SetTemporaryFeedbackMessage(
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

            var resetLink = Url.Action("ResetPassword", "Account",
            new { userId = user.Id, token = resetToken },
            Request.Scheme);

            await _emailService.SendPasswordReset(
                new EmailAddress(model.Email),
                user.UserName,
                resetLink);

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

                    Regex.Split(error.Description, @"(?<=[.])").ToList().ForEach(e =>
                    {
                        if (!string.IsNullOrWhiteSpace(e))
                            htmlMessage += $"<li>{e}</li>";
                    });

                    htmlMessage += "</ul>";

                    this.SetTemporaryFeedbackMessage(htmlMessage, FeedbackMessageType.Danger, true);
                    return View(model);
                }

                await _tokenLog.ValidationSuccessful(tokenValidationId);
            }
            catch (Exception e)
            {
                this.SetTemporaryFeedbackMessage(e.Message, FeedbackMessageType.Danger);
                return View(model);
            }

            //They've reset their password - we sign them in!
            var signInStatus = await _signinManager.PasswordSignInAsync(user, model.Password, false, false);

            if (signInStatus.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (signInStatus.IsLockedOut)
            {
                this.SetTemporaryFeedbackMessage("This account has been locked out. Please wait and try again later.", FeedbackMessageType.Danger);
            }
            else if (signInStatus.IsNotAllowed)
            {
                var supportEmail = _siteConfig.SupportAddress;
                this.SetTemporaryFeedbackMessage(
                    "This account has not been confirmed. " +
                    $"You can <a href=\"{Url.Action("ResendConfirmLink", new { userEmail = user.Email, returnUrl = Url.Action("Login") })}\">resend your confirmation link</a>, " +
                    $"or contact <a href=\"mailto:{supportEmail}\">{supportEmail}</a> " +
                    "if you're having trouble.",
                    FeedbackMessageType.Warning,
                    true);
            }
            else
            {
                this.SetTemporaryFeedbackMessage("Email / password incorrect. Please try again.", FeedbackMessageType.Danger);
            }
            return View(model);          
        }

        #endregion

        #region Access Control

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            var supportEmail = _siteConfig.SupportAddress;

            this.SetTemporaryFeedbackMessage("Access to the requested page was denied. If you think this is an error " +
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
        
        #region Session Management
        
        public JsonResult KeepSessionAliveAjax()
        {
            return Json(new
            {
                success = true
            });
        }
        
        #endregion

    }
}
