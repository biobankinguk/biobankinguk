using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Data.Identity;
using Directory.Auth.Identity;
using Directory.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

namespace Directory.Pages.Account
{
    public class ConfirmModel : PageModel
    {
        private readonly DirectoryUserManager _users;
        private readonly TokenLoggingService _tokenLog;
        private readonly IConfiguration _config;
        private readonly SignInManager<DirectoryUser> _signIn;

        public ConfirmModel(
            DirectoryUserManager users,
            SignInManager<DirectoryUser> signIn,
            TokenLoggingService tokenLog,
            IConfiguration config)
        {
            _users = users;
            _tokenLog = tokenLog;
            _config = config;
            _signIn = signIn;
        }

        public async Task<IActionResult> OnGet(string? userId, string? code)
        {
            var generalError = "The User ID or Token is invalid or has expired.";

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError(string.Empty, generalError);
                return Page();
            }

            await _tokenLog.AccountConfirmationTokenValidationAttempted(code, userId);

            var user = await _users.FindByIdAsync(userId);
            if (user is null)
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(new[] {
                        new { Code = "InvalidUserId", Description = "Invalid User ID" }
                    }));

                ModelState.AddModelError(string.Empty, generalError);
                return Page();
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _users.ConfirmEmailAsync(user, code);

            if (result.Errors.Any())
            {
                await _tokenLog.AccountConfirmationTokenValidationFailed(
                    code, userId, JsonSerializer.Serialize(result.Errors));

                ModelState.AddModelError(string.Empty, generalError);
            }
            else
            {
                await _signIn.SignInAsync(user, false);
                await _tokenLog.AccountConfirmationTokenValidationSuccessful(code, userId);
            }

            return Page();
        }
    }
}
