using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.ADAC;
using Biobanks.Web.Utilities;
using MvcSiteMapProvider.Web.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Biobanks.Web.Controllers
{
    [Obsolete("To be deleted when the Directory core version goes live." +
    " Any changes made here will need to be made in the corresponding core version"
    , false)]
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
            var pages = await _contentPageService.ListContentPages();

            return View(pages.Select(x => new ContentPageModel
            {
                Id = x.Id,
                Title = x.Title,
                RouteSlug = x.RouteSlug,
                LastUpdated = x.LastUpdated,
                IsEnabled = x.IsEnabled
            }));
        }

        [AllowAnonymous]
        [SiteMapTitle("Title")]
        public async Task<ActionResult> ContentPage(string slug)
        {
            var page = await _contentPageService.GetBySlug(slug);

            if (page != null)
            {
                if (page.IsEnabled || User.IsInRole("ADAC"))
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
            }

            return RedirectToAction("Index", "Home");
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
                    return RedirectToAction("Index", "Home");

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
                return RedirectToAction("Index", "Home");

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

        public ActionResult CreateEditPreview()
            => Redirect("Index");

        [HttpPost]
        public ActionResult CreateEditPreview(ContentPageModel page)
            => View("CreateEditPreview", page);

        [HttpPost]
        public async Task<ActionResult> Submit(ContentPageModel page)
        {
            // Create
            if (page.Id == 0)
            {
                if ((await _contentPageService.GetBySlug(page.RouteSlug)) != null)
                {
                    SetTemporaryFeedbackMessage($"A content page with the slug '{page.RouteSlug}' already exists.", FeedbackMessageType.Warning);
                }
                else
                {
                    await _contentPageService.Create(page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                    SetTemporaryFeedbackMessage($"The content page '{page.Title}' has been created successfully.", FeedbackMessageType.Success);
                }

                return RedirectToAction("Index", "PagesAdmin");
            }
            // Edit
            else
            {
                var existingPage = await _contentPageService.GetById(page.Id);

                //Checking if the user is changing the slug value and if the new value already exists
                if (existingPage.RouteSlug != page.RouteSlug && (await _contentPageService.GetBySlug(page.RouteSlug)) != null)
                {
                    SetTemporaryFeedbackMessage($"A content page with the slug '{page.RouteSlug}' already exists.", FeedbackMessageType.Warning);
                }
                else
                {
                    await _contentPageService.Update(page.Id, page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                    SetTemporaryFeedbackMessage($"The content page '{page.Title}' has been edited successfully.", FeedbackMessageType.Success);
                }

                return RedirectToAction("Index", "PagesAdmin");
            }
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            var pageName = (await _contentPageService.GetById(id)).Title;
            await _contentPageService.Delete(id);
            SetTemporaryFeedbackMessage($"The content page '{pageName}' has been deleted successfully.", FeedbackMessageType.Success);
            return Json(new
            {
                success = true,
                name = pageName
            });
        }

    }
}