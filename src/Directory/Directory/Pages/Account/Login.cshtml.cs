using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Auth.IdentityServer;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Account
{
    public class LoginModel : BaseReactModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IEventService _events;
        private readonly DirectoryUserManager _users;
        private readonly SignInManager<DirectoryUser> _signIn;

        public LoginModel(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IEventService events,
            DirectoryUserManager userManager,
            SignInManager<DirectoryUser> signInManager)
            : base(ReactRoutes.Login)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _events = events;
            _users = userManager;
            _signIn = signInManager;
        }

        [BindProperty]
        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? RedirectUrl { get; set; }
        public bool AllowResend { get; set; }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            // Technically we should check whether the Client
            // a) Allows local login
            // b) Allows any external providers for login
            // But currently we know that we only allow local login
            // so all valid clients must allow it and have no external providers.
            //
            // This may change, and if it does should be checked and recorded on this ViewModel

            // if we've come from a client, we may want to pre-populate an expected username
            Username = context?.LoginHint ?? string.Empty;
            return Page();
        }

        /// <summary>
        /// Handles login form POSTs, whether the submission was
        /// Login OR Cancel.
        /// </summary>
        /// <param name="button">Which form button was pressed; Login or Cancel</param>
        /// <param name="returnUrl">Optional Return URL</param>
        public async Task<IActionResult> OnPostAsync(string button, string? returnUrl = null)
        {
            // Check that we're in the context of an autho request
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            if (button != "login") // login form cancelled
            {
                if (context is null) return Redirect("~/");

                // if the user cancels, send a result back into IdentityServer as if they 
                // denied the consent (even if this client does not require consent).
                // this will send back an access denied OIDC error response to the client.
                await _interaction.GrantConsentAsync(context, ConsentResponse.Denied);

                return await ContextAwareRedirect(context, returnUrl);
            }

            if (ModelState.IsValid)
            {
                // Validate credentials
                var result = await _signIn.PasswordSignInAsync(Username, Password, false, true);

                var user = await _users.FindByNameAsync(Username);

                if (result.Succeeded)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(
                        user.UserName,
                        user.Id,
                        user.Name,
                        clientId: context?.ClientId));

                    await _users.UpdateLastLogin(user);

                    if (context is { }) return await ContextAwareRedirect(context, returnUrl);

                    return returnUrl switch
                    {
                        var url when Url.IsLocalUrl(url) => Redirect(returnUrl),
                        var url when string.IsNullOrEmpty(url) => Redirect("~/"),
                        _ => throw new InvalidOperationException("Invalid Return URL")
                    };
                }

                // handle various failure scenarios appropriately

                string eventError = "Login failure";
                string friendlyError = "The username and/or password are invalid, or otherwise not allowed.";

                if (result.IsLockedOut)
                {
                    eventError = "Account locked out";
                    friendlyError = "This account is currently locked out. Please try again later.";
                }

                if (result.RequiresTwoFactor)
                    throw new NotImplementedException("2 Factor Authentication is not yet handled.");

                // all other disallowed cases
                if (result.IsNotAllowed)
                {
                    if (user is { } && !user.EmailConfirmed)
                        AllowResend = true;

                    eventError = "Credentials not allowed";
                }

                // log the failure and set model state errors if appropriate
                await _events.RaiseAsync(new UserLoginFailureEvent(
                        Username,
                        error: eventError,
                        clientId: context?.ClientId));
                ModelState.AddModelError(string.Empty, friendlyError);
            }

            // Something went wrong
            return Page();
        }

        private async Task<IActionResult> ContextAwareRedirect(AuthorizationRequest context, string? returnUrl)
        {
            var url = returnUrl ?? "~/";

            if (await _clientStore.IsPkceClientAsync(context.ClientId))
            {
                // if the client is PKCE then we assume it's native, so this change in how to
                // return the response is for better UX for the end user.
                Route = ReactRoutes.LoginRedirect;
                RedirectUrl = url;
                return Page();
            }

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(url);
        }
    }
}