using System.Diagnostics;
using System.Linq;
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

        public string Index() => "go to `/Home/Identity` to test authorising against a protected route using the IdP";


        [Authorize]
        public async Task<IActionResult> Identity()
            => Json(new
            {
                Claims = User.Claims.Select(c => new { c.Type, c.Value }),
                Properties = (await HttpContext.AuthenticateAsync())
                    .Properties.Items.Select(p => new { p.Key, p.Value })
            });

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
