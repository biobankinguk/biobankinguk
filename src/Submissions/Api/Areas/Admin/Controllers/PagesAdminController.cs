using Biobanks.Submissions.Api.Services.Directory.Contracts;
using Biobanks.Submissions.Api.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Biobanks.Submissions.Api.Areas.Admin.Models.PagesAdmin;
using Biobanks.Submissions.Api.Auth;
using cloudscribe.Web.Navigation;

namespace Biobanks.Submissions.Api.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(nameof(AuthPolicies.IsDirectoryAdmin))]
public class PagesAdminController : Controller
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
    public async Task<ActionResult> ContentPage(string slug)
    {
        var page = await _contentPageService.GetBySlug(slug);

        if (page != null)
        {
            if (page.IsEnabled || User.IsInRole("Admin"))
            {
                // Set the last breadcrumb
                var breadCrumbHelper = new TailCrumbUtility(HttpContext);
                breadCrumbHelper.AddTailCrumb(page.Title, page.Title, "");
                
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
                this.SetTemporaryFeedbackMessage($"A content page with the slug '{page.RouteSlug}' already exists.", FeedbackMessageType.Warning);
            }
            else
            {
                await _contentPageService.Create(page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                this.SetTemporaryFeedbackMessage($"The content page '{page.Title}' has been created successfully.", FeedbackMessageType.Success);
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
                this.SetTemporaryFeedbackMessage($"A content page with the slug '{page.RouteSlug}' already exists.", FeedbackMessageType.Warning);
            }
            else
            {
                await _contentPageService.Update(page.Id, page.Title, page.Body, page.RouteSlug, page.IsEnabled);
                this.SetTemporaryFeedbackMessage($"The content page '{page.Title}' has been edited successfully.", FeedbackMessageType.Success);
            }

            return RedirectToAction("Index", "PagesAdmin");
        }
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(int id)
    {
        var pageName = (await _contentPageService.GetById(id)).Title;
        await _contentPageService.Delete(id);
        this.SetTemporaryFeedbackMessage($"The content page '{pageName}' has been deleted successfully.", FeedbackMessageType.Success);
        return Ok(new
        {
            success = true,
            name = pageName
        });
    }
}

