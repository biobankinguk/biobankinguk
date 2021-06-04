using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Models.Shared;
using MvcSiteMapProvider.Web.Mvc.Filters;
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
                    Title = x.Title,
                    RouteSlug = x.RouteSlug
                }));            
        }

        [AllowAnonymous]
        [SiteMapTitle("Title")]
        public async Task<ActionResult> ContentPage(string slug)
        {
            var page = await _contentPageService.GetBySlug(slug);

            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!page.IsEnabled)
            {
                if (User.IsInRole("ADAC"))
                {
                    return View(new ContentPageModel
                    {
                        Id = page.Id,
                        Title = page.Title,
                        Body = page.Body,
                        RouteSlug = page.RouteSlug,
                        LastUpdated = page.LastUpdated
                    });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }            
            }

            return View(new ContentPageModel
            {
                Id = page.Id,
                Title = page.Title,
                Body = page.Body,
                RouteSlug = page.RouteSlug,
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
                    Id = page.Id,
                    Title = page.Title,
                    Body = page.Body,
                    RouteSlug = page.RouteSlug,
                    LastUpdated = page.LastUpdated,
                    IsEnabled = page.IsEnabled
                });
            }
        }

        [HttpPost]
        public ActionResult CreateEdit(ContentPageModel page)
        {

            if (page == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new ContentPageModel
            {
                Id = page.Id,
                Title = page.Title,
                Body = page.Body,
                RouteSlug = page.RouteSlug,
                LastUpdated = page.LastUpdated,
                IsEnabled = page.IsEnabled
            });
            
        }


        [HttpPost]
        public async Task<ActionResult> Submit(ContentPageModel page)
        {
            // Create
            if (page.Id == 0)
            {
                await _contentPageService.Create(page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                return RedirectToAction("Index", "PagesAdmin");
            }
            // Edit
            else
            {
                await _contentPageService.Update(page.Id, page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                return RedirectToAction("Index", "PagesAdmin");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var pageName = (await _contentPageService.GetById(id)).Title;
            await _contentPageService.Delete(id);

            return Json(new
            {
                success = true,
                name = pageName
            });
        }

        public ActionResult CreateEditPreview()
         => Redirect("Index");

        [HttpPost]
        public ActionResult CreateEditPreview(ContentPageModel page)
            => View("CreateEditPreview", page);

    }
}