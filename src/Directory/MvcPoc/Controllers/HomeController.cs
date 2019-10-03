using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcPoc.Models;

namespace MvcPoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public string Index() => "`/Home/Identity` to test interactive login; `/Home/Logout` to Sign Out. `/Home/CallApi` to test token api access";

        [Authorize]
        public async Task<IActionResult> Identity()
            => Json(new
            {
                Claims = User.Claims.Select(c => new { c.Type, c.Value }),
                Properties = (await HttpContext.AuthenticateAsync())
                    .Properties.Items.Select(p => new { p.Key, p.Value })
            });

        [Authorize]
        public async Task<IActionResult> CallApi()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            return Ok(await client.GetStringAsync("https://localhost:5001/identity"));
        }

        [Authorize]
        public IActionResult Login() =>
            // this is kind of optional as going any protected route triggers a login
            // but if you wanted a Login link, this route would work
            // because it has an AuthorizeAttribute
            Redirect("~/");

        public IActionResult Logout() => SignOut("Cookies", "oidc");

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
