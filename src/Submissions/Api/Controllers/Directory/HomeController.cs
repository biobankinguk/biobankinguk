using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Controllers.Directory
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
