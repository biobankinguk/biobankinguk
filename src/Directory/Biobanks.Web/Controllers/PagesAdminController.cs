using AutoMapper;
using Biobanks.Services.Contracts;
using Biobanks.Web.Filters;
using Biobanks.Web.Models.ADAC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
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
            /*return View((await _contentPageService.ListContentPages())
                .Select(x => new ContentPageModel
                {
                    Id = x.Id,
                    Title = x.Title
                }));*/
            var contentPages = (await _contentPageService.ListContentPages())
                .Select(x =>
                    Task.Run(async () => new ContentPageModel()
                    {
                        Id = x.Id,
                        Title = x.Title
                    })
                    .Result
                )
                .ToList();

            return View(new ContentPagesModel()
            {
                ContentPages = contentPages
            });
        }
    }
}