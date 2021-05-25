using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.ADAC;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    [UserAuthorize(Roles = "ADAC")]
    public class PagesAdminController : ApplicationBaseController
    {
        private readonly IContentPageService _contentPageService;        

        public PagesAdminController(IContentPageService contentPageService)
        {
            _contentPageService = contentPageService;
        }

        public async Task<ActionResult> Index()
        {
            return View((await _contentPageService.ListContentPages())
                .Select(x => new ContentPageModel
                {
                    Id = x.Id,
                    Title = x.Title
                }));            
        }

        public async Task<ActionResult> ContentPage(string slug)
        {
            var page = await _contentPageService.GetBySlug(slug);

            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new PageModel
            {
                Title = page.Title,
                Body = page.Body,
                LastUpdated = page.LastUpdated
            });
        }
    }
}