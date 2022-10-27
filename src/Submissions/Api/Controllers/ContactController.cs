using Microsoft.AspNetCore.Mvc;

namespace Biobanks.Submissions.Api.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
