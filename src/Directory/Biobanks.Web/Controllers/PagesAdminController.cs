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

            return View(new ContentPageModel
            {
                Title = page.Title,
                Body = page.Body,
                LastUpdated = page.LastUpdated
            });
        }

        [HttpGet]
        public async Task<ActionResult> CreateEdit(int id)
        {
            // Create
            if (id == 0)
            {
                return View(new ContentPageModel());
            }
            // Edit
            else
            {
                var page = await _contentPageService.GetById(id);

                if (page == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View(new ContentPageModel
                {
                    Title = page.Title,
                    Body = page.Body,
                    RouteSlug = page.RouteSlug
                });
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateEdit(ContentPageModel page)
        {
            // Create
            if (page.Id == 0)
            {
                await _contentPageService.Create(page.Title, page.Body, page.RouteSlug);
                return RedirectToAction("Index", "PagesAdmin");
            }
            // Edit
            else
            {
                await _contentPageService.Update(page.Id, page.Title, page.Body, page.RouteSlug);
                return RedirectToAction("Index", "PagesAdmin");
            }
        }
    }
}