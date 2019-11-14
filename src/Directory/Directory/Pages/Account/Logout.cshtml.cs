using System.Threading.Tasks;
using Common.Data.Identity;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Directory.Pages.Account
{
    public class LogoutModel : BaseReactModel
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly SignInManager<DirectoryUser> _signIn;

        public LogoutModel(
            IIdentityServerInteractionService interaction,
            IEventService events,
            SignInManager<DirectoryUser> signIn)
            : base(ReactRoutes.LogoutConfirm)
        {
            _interaction = interaction;
            _events = events;
            _signIn = signIn;
        }

        [BindProperty]
        [NoJsonViewModel]
        public string? LogoutId { get; set; }

        public string? PostLogoutRedirectUri { get; set; }
        public string? ClientName { get; set; }

        [NoJsonViewModel]
        public string? SignOutIframeUrl { get; set; }

        public async Task<IActionResult> OnGet(string? logoutId = null)
        {
            LogoutId = logoutId;

            return await ShowSignoutPrompt()
                ? Page()
                : await OnPost();
        }

        private async Task<bool> ShowSignoutPrompt()
        {
            // no authenticated user - no prompt
            if (User is null || !User.Identity.IsAuthenticated)
                return false;

            // context doesn't require prompt
            var context = await _interaction.GetLogoutContextAsync(LogoutId);
            if (context?.ShowSignoutPrompt is false)
                return false;

            return true;
        }

        public async Task<IActionResult> OnPost()
        {
            var logout = await _interaction.GetLogoutContextAsync(LogoutId);

            // If external IdP's are supported, we should check the user
            // to see which providers/schemes we should be signing out of
            // but for now, we don't use this functionality

            if (User?.Identity.IsAuthenticated is true)
            {
                await _signIn.SignOutAsync();

                await _events.RaiseAsync(new UserLogoutSuccessEvent(
                    User.GetSubjectId(),
                    User.GetDisplayName()));
            }

            // Here's where we'd trigger external signouts if there were any

            // Set Model props and return the Page
            ClientName = GetClientName(logout);
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri;
            SignOutIframeUrl = logout?.SignOutIFrameUrl;

            return Page(ReactRoutes.LogoutRedirect);
        }

        private static string? GetClientName(LogoutRequest? logout)
            => string.IsNullOrWhiteSpace(logout?.ClientName)
                ? logout?.ClientId
                : logout?.ClientName;
    }
}
